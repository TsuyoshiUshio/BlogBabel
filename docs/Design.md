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
BlogBabel.exe --Source [Source Provider Name] --Source-Id [Source Blog Post ID] --Dest [Destination Provider Name]
```

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

### Dev.to

* [API](https://developers.forem.com/api/v1#tag/articles/operation/createArticle)
* [API Key](https://dev.to/settings/extensions)

We can use `API Key` for accessing the API.

### OpenAI and SDK

* [OpenAI documentation](https://platform.openai.com/docs/introduction)
* [Azure OpenAI SDK](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI)

### System.Text.Json

Use JsonNode for parsing.

* [JsonNode](https://learn.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json/use-dom#use-jsonnode)