# BabelBlog Design Document

BabelBlog is a tool designed to translate blogs between two languages, maintaining the original context, nuance, and document format (e.g., Markdown). This document outlines the motivation, architecture, and features of BabelBlog.

## Motivation

As a bilingual blogger, I often write technical blogs in both English and Japanese. However, translating these blogs manually is time-consuming and machine translation often loses the original context and nuance. BabelBlog aims to solve this problem by providing a more efficient and accurate translation solution.

## Key Benefit 

**Efficiency:** BabelBlog automates the process of translating blogs, saving the user time and effort.

**Accuracy:** BabelBlog aims to maintain the original context and nuance of the blog post during translation, which can be a challenge with manual translation or other machine translation tools.

**Convenience:** BabelBlog is a command-line tool that can be easily integrated into the user's workflow. It also supports various blog platforms, providing flexibility for the user.

**Preview Feature:** Before publishing, BabelBlog posts a preview of the translated blog post on the destination platform. This allows the user to review and make any necessary adjustments to the translation.

**Customization:** BabelBlog allows users to configure provider-specific settings such as language and sentiment, providing a more tailored translation experience.

## Architecture 

BabelBlog is a C# command-line tool that supports Windows, Linux, and Mac. It uses the OpenAI REST API and supports various blog platforms, starting with [Qiita](https://qiita.com) and [Dev.Community](https://dev.to). The tool fetches a source blog post, creates a translated version on a destination blog service, and includes links to the original post and the BabelBlog project page. Future plans include support for [Azure Functions OpenAI extensions](https://github.com/cgillum/azure-functions-openai-extension) for automatic translation upon blog modification.

### Chunk translation

GTP-4 turbo can handle [128,000 token](https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo) However, return token is limited to `4096` token. That means We can not translate it in one time.
We are going to separate original document into several chunks. We can use [Tokenizer](https://platform.openai.com/tokenizer) that calculate the number of tokens.
The tokenizer is TypeScript library run in locally. We need to convert it into C#. Currently, we can find [C# Tokenizer](https://github.com/betalgo/openai/blob/master/OpenAI.SDK/Tokenizer/GPT3/TokenizerGpt3.cs), however, it doesn't support new model that we are going to use. We decide that convert [tiktoken](https://github.com/dqbd/tiktoken/blob/main/js/src/core.ts) on TypeScript into C# implementation.
The tiktoken is based on [Byte pair encoding(BPE)](https://github.com/openai/tiktoken/tree/main?tab=readme-ov-file#what-is-bpe-anyway).

Finally we found [Microsoft/Tokenizer](https://github.com/microsoft/Tokenizer/tree/main) that is officially supported.

### Translation process

We have 5 processes to make the translation successfully.

#### 1. Initialize

Connect to the Providers (e.g. QiitaProvider, OpenAIProvider, DevToProvider) by reading the config file.

#### 2. Load

Load from the Source provider. Provider can be Source/Destination provider.

#### 3. PreProcessing

Check the total Token Count and separate with chank that contains token less than 4096.
The algorithm will be:
* Get the token count and devided to chanks within 4096. We find when it exceed 2048, find the line start with `#` or exceed 3000 token.  
* Read the document from top until it reach to the limit. (e.g. if the limit is 90 %, the read line until it reaches header in between 90% to 100%), until it finsihed reading

#### 4. Processing

Translate the chunk using OpenAI for each.
Summarize the body of the post.

#### 5. Publish

Publish the translation into Desination provider as a preview mode. 

#### Development Strategy

Start with Simple Logic with Providers, then refactor the sofisticated design.

## Features

### Translation

BabelBlog can be run from the command line with the following options:

```powershell
BlogBabel.exe --Source Qiita --Source-Id 2543 --Dest DevTo

### Configuration

BabelBlog configuration is stored in a JSON file and includes provider-specific settings such as language, sentiment, and credentials:

```json:config.json
{
    "Providers": {
        "Qiita": {
            // Credential settings (e.g. Connection String/Token/ApiKey)
            "Language": "Japanese",
            "Sentiment": "Technical Blog"  // Configuration parameter will be TBD after spike solutions.
        },
        "DevTo": {
            // Credential settings
        }
    }
}
```

## How to Use BabelBlog

### 1. Write Your Blog Post
Start by writing your blog post as you normally would.

### 2. Configure BabelBlog
Next, configure BabelBlog by editing the `config.json` file. This file should include your credential settings and other provider-specific settings such as language and sentiment.

```json
{
    "Providers": {
        "Qiita": {
            // Credential settings (e.g. Connection String/Token/ApiKey)
            "Language": "Japanese",
            "Sentiment": "Technical Blog"  // Configuration parameter will be TBD after spike solutions.
        },
        "DevTo": {
            // Credential settings
        }
    }
}
```

### 3. Run BabelBlog

Once your blog post is written and BabelBlog is configured, you can run BabelBlog with the following command:

```powershell
BlogBabel.exe --Src [Source Provider Name] --Src-Id [Source Blog Post ID] --Dest [Destination Provider Name]
```

#### Available parameters

| Parameter | Description | Required | Default |
| --------- | ----------- | -------- | ------- |
| Src | Source Blog Provider (e.g. Qiita, DevTo) | true | - |
| Src-Id | The identifier of the Source Blog post. | true | - |
| Dest | The blog that will create a new post with translation | true | - |
| Verbose | Show the all trace logging | false | false |
| Model | Specify the GPT model (e.g. gpt4-turbo) | false | gpt3-turbo |
| TokenLimit | Specify the Token Limit that start spliting post into chunks | false | 2000 |
| MaxTokenLimit | Specify the Max Token that is used for a chunk | false | 3000 |


This command will read your original blog post, translate it, and post a preview of the translated post on the destination platform.

### 4. Review and Publish

Finally, review the translated blog post on the destination platform. If everything looks good, you can publish it. You can find the preview blog post at the following URL:

```powershell
[BlogTitle]: https://[Destination Provider URL]/[translated blog post uri]
```

## Resources

### Qiita

* [API](https://qiita.com/api/v2/docs#%E6%A6%82%E8%A6%81)
* [Application](https://qiita.com/settings/applications)

We choose `Personal Access Token` for accessing Qiita. We can choose OAuth2.0 with flow that include redirect. However, it is for website usage. It is too much for creating web site for this purpose.

Payload

```json
body
required
Item body in Markdown
Example: "# Example"
Type: string

coediting
A flag whether this item is co-edit mode (only available on Qiita Team)
Example: false
Type: boolean

group_url_name
A group's url_name on which share this item (pass null to make public; only available on Qiita Team)
Example: "dev"
Type: null, string

private
A flag whether this item is private (only available on Qiita)
Example: false
Type: boolean

tags
required
A list of tags
Example: [{"name"=>"Ruby", "versions"=>["0.0.1"]}]
Type: array

title
required
The title of this item
Example: "Example title"
Type: string

tweet
A flag to post a tweet (only availabble if Twitter integration is enabled)
Example: false
Type: boolean

organization_url_name
Represents url_name of Organization of this item.
Example: "qiita-inc"
Type: string, null

slide
A flag whether this item has slide mode enabled
Example: false
Type: boolean
```

Qiita identifier is `item_id` that found on the url of the blog post. 
Following is the item API for get request.

```
GET /api/v2/items/:item_id HTTP/1.1
Host: api.example.com
```

## Sample CommandLine arguments

```
--src Qiita --dest DevTo  --src-id b42773afaa4a25c2af60
```

### Dev.to

* [API](https://developers.forem.com/api/v1#tag/articles/operation/createArticle)
* [API Key](https://dev.to/settings/extensions)

We can use `API Key` for accessing the API.

Payload

```json
{
    "article": {
    "title": "string",
    "body_markdown": "string",
    "published": false,
    "series": "string",
    "main_image": "string",
    "canonical_url": "string",
    "description": "string",
    "tags": "   ",
    "organization_id": 0
    }
}
```

Read article requires `id` or `path`.  Users can't identity the `id` so that 
I'd like to use the path for the identifier.  The articles endpoint is following.

```
GET https://dev.to/api/articles/{username}/{slug}
```

## Sample CommandLine arguments

```
--src DevTo --dest Qiita  --src-id tsuyoshiushio/kafka-scaledjobs-sample-for-keda-with-eventhubs-h2m
```

### Exchange the post

The providers has different payload. We are focusing small part.
I use `Dev.to` as the tipcial formatting. 

`title`: translate `title` from another provider
`body_markdown`: translate `body_markdown` equivalent from another provider
`published`: Start with `false`. We need to review the translation before publishing it
`tags`: After the translate has done, AI can find proper tags and pass it. If it exists, ai will translate it.

Any other properties are not important and scope out from the first impelmentation.

### OpenAI and SDK

* [OpenAI documentation](https://platform.openai.com/docs/introduction)
* [Azure OpenAI SDK](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI)

### Microsoft Tokenizer

* [Microsoft Tokenizer](https://github.com/microsoft/Tokenizer/tree/main)

### System.Text.Json

Use JsonNode for parsing.

* [JsonNode](https://learn.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json/use-dom#use-jsonnode)

## Trouble Shooting

### DevTo

Returns 403. I was missing User-Agent that is required.
* [Getting forbidden bots error message on Nodejs web server](https://github.com/forem/forem/issues/15822)

Returns 422. Usually missing required parameters.
Another case is, OpenAI returns message for `tags`. I modify the prompt not return the message.
