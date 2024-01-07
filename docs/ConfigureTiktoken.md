# Configure Tiktoken

Explains how to set up js-tiktoken.

## Clone https://github.com/dqbd/tiktoken 

Clone https://github.com/dqbd/tiktoken on WSL. 

## Update node.js to 18

Update the node.js to version 18 by following [Debian and Ubuntu based distributions](https://github.com/nodesource/distributions?tab=readme-ov-file#debian-and-ubuntu-based-distributions).

```bash
 curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash - &&sudo apt-get install -y nodejs
``````

I encountered the following issue. 

```bash
dpkg: error processing archive /var/cache/apt/archives/nodejs_18.19.0-1nodesource1_amd64.deb (--unpack):
 trying to overwrite '/usr/include/node/common.gypi', which is also in package libnode-dev 12.22.9~dfsg-1ubuntu3
dpkg-deb: error: paste subprocess was killed by signal (Broken pipe)
Errors were encountered while processing:
 /var/cache/apt/archives/nodejs_18.19.0-1nodesource1_amd64.deb
```

I ran following command by following [dpkg error: "trying to overwrite file, which is also in..."](https://askubuntu.com/questions/176121/dpkg-error-trying-to-overwrite-file-which-is-also-in).

The error message suggests that the package is trying to overwrite a file that is already present in another package.  This is a common issue that can be resolved by using the --force-overwrite option with the dpkg command.

```bash
sudo dpkg -i --force-overwrite /var/cache/apt/archives/nodejs_18.19.0-1nodesource1_amd64.deb
sudo apt-get --fix-broken install
sudo apt-get update && sudo apt-get upgrade
```

Then encountered

```bash
Errors were encountered while processing:
 /var/cache/apt/archives/libnode-dev_12.22.9~dfsg-1ubuntu3.3_amd64.deb
 /var/cache/apt/archives/libnode72_12.22.9~dfsg-1ubuntu3.3_amd64.deb
E: Sub-process /usr/bin/dpkg returned an error code (1)
```

It seems that you are encountering an error code 1 while trying to install libnode-dev and libnode72 packages. This error code is usually associated with a dpkg error 1. Here are some steps that you can try to resolve this issue:

```bash
sudo dpkg -r libnode-dev libnode72
sudo apt-get --fix-broken install
sudo apt-get update && sudo apt-get upgrade
```
It fixed on my environment.

## Install software

Install [yarn](https://classic.yarnpkg.com/lang/en/docs/install/#windows-stable) and  [wasm](https://rustwasm.github.io/wasm-pack/installer/).

* Update npm
* Install yarn
* Install [Rust](https://rustup.rs/)
* [WSL2 extension](https://code.visualstudio.com/blogs/2019/09/03/wsl2)

```bash
sudo npm install -g npm@10.2.5
sudo npm install --global yarn
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"
curl https://rustwasm.github.io/wasm-pack/installer/init.sh -sSf | sh
```

This command generate the files of ranks. We need to analyze the ranks files.

```bash
// build essential is required for c compiler.
sudo apt update && sudo apt install build-essential
yarn install --frozen-lockfile
yarn run build
yarn run test
```

For debgging, [compatibility.test.ts](https://github.com/dqbd/tiktoken/blob/main/js/test/compatibility.test.ts) file use following extensions. 
* [Vitest explorer](https://marketplace.visualstudio.com/items?itemName=ZixuanChen.vitest-explorer)
* [Vitest Runner](https://marketplace.visualstudio.com/items?itemName=kingwl.vscode-vitest-runner)

The extension enables you to debug without any configration.
