[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![BlogBabel Build and Tests](https://github.com/TsuyoshiUshio/BlogBabel/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/TsuyoshiUshio/BlogBabel/actions/workflows/build-and-test.yml)
[![BlogBabel Release](https://github.com/TsuyoshiUshio/BlogBabel/actions/workflows/release.yml/badge.svg)](https://github.com/TsuyoshiUshio/BlogBabel/actions/workflows/release.yml)

# BlogBabel

BlogBabel is a tool designed to translate blog posts written in one language into another, powered by OpenAI's REST API and LLM.

## Motivation

As a bilingual blogger, the need to write posts in both English and Japanese can be time-consuming. BlogBabel aims to reduce this workload by automating the translation process.

## Architecture

For a detailed explanation of the architecture, please refer to the [Design Document](docs/Design.md).

## Usage

To use BlogBabel:

1. Download the corresponding binary for your operating system.
2. Run the command.

For example, the following command translates an article on `Qiita` written in `Japanese` and posts it on `DevTo` in `English`:

```powershell
BlogBabel.exe --src Qiita --dest DevTo --src-id b42773afaa4a25c2af60
```

For more options, refer to the [DesignDoc](docs/Design.md)

# Contributing

Contributions are welcome! Feel free to send a PR to this repo. The project is developed using Visual Studio 2022 with C#8. We are particularly interested in contributions for providers, such as `QiitaProvider` and `DevToProvider`, especially for popular blog servers in `Spanish`, `French`, `German`, `Korean`, `Chinese` and many more! We recommend finding a blog service that supports the `Markdown` format, as this makes it easier to translate while maintaining the structure.

# Feature Plans

We plan to implement a custom GitHub Actions to run `BlogBabel` wihtout needing to install it on your machine.
