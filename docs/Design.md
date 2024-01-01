# BabelBlog Design Document

BabelBlog is a tool designed to translate blogs between two languages, maintaining the original context, nuance, and document format (e.g., Markdown). This document outlines the motivation, architecture, and features of BabelBlog.

## Motivation

As a bilingual blogger, I often write technical blogs in both English and Japanese. However, translating these blogs manually is time-consuming and machine translation often loses the original context and nuance. BabelBlog aims to solve this problem by providing a more efficient and accurate translation solution.

## Architecture 

BabelBlog is a C# command-line tool that supports Windows, Linux, and Mac. It uses the OpenAI REST API and supports various blog platforms, starting with [Qiita](https://qiita.com) and [Dev.Community](https://dev.to). The tool fetches a source blog post, creates a translated version on a destination blog service, and includes links to the original post and the BabelBlog project page. Future plans include support for [Azure Functions OpenAI extensions](https://github.com/cgillum/azure-functions-openai-extension) for automatic translation upon blog modification.

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