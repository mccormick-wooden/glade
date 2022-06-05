
---
# Unity CI with Github Actions
---

1. in project root, `mkdir .github && mkdir .github/workflows && cd .github/workflows && touch activation.yml`
2. Add the following to `activation.yml` (sourced from [here](https://game.ci/docs/github/activation))

```yml
name: Acquire activation file
on:
  workflow_dispatch: {}
jobs:
  activation:
    name: Request manual activation file 🔑
    runs-on: ubuntu-latest
    steps:
      # Request manual activation file
      - name: Request manual activation file
        id: getManualLicenseFile
        uses: game-ci/unity-request-activation-file@v2
      # Upload artifact (Unity_v20XX.X.XXXX.alf)
      - name: Expose as artifact
        uses: actions/upload-artifact@v2
        with:
          name: ${{ steps.getManualLicenseFile.outputs.filePath }}
          path: ${{ steps.getManualLicenseFile.outputs.filePath }}
```

3. commit and push
4. In repo navigate to `Actions > Acquire activation file > Run workflow`, and run the workflow to acquire the activation file
5. When workflow completes, click `Acquire activation file` and download the archive
	- Should be named something like `Unity_v2019.2.11f1.alf`
6. Extract the alf file and upload [here](https://license.unity3d.com/manual). Follow the instructions to acquire the ulf file
7. Once you have the ulf file, in Github go to `Settings > Secrets > Actions` and copy the file contents into a new repository secret called `UNITY_LICENSE` and add the secret to the repo
10. From project root, `touch .github/workflows/main.yml`
11. Add the following to `main.yml` (modified slightly from source [here](https://game.ci/docs/github/builder))

```yml
name: Glade Build

on: [push, pull_request]

jobs:
  buildForAllSupportedPlatforms:
    name: Build for ${{ matrix.targetPlatform }}
    runs-on: ubuntu-latest
    strategy:
      fail-fast: true
      matrix:
        targetPlatform:
          - StandaloneOSX # Build a macOS standalone (Intel 64-bit).
          - StandaloneWindows64 # Build a Windows 64-bit standalone.
    steps:
      - uses: actions/checkout@v2
        with:
          fetch-depth: 0
          lfs: true
      - uses: actions/cache@v2
        with:
          path: Library
          key: Library-${{ matrix.targetPlatform }}
          restore-keys: Library-
      - uses: game-ci/unity-builder@v2
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
        with:
          targetPlatform: ${{ matrix.targetPlatform }}
          unityVersion: auto
      - uses: actions/upload-artifact@v2
        with:
          name: Build-${{ matrix.targetPlatform }}
          path: build/${{ matrix.targetPlatform }}
```

10. Copy build script into file and push
11. Build should now run!
---
# References
https://www.youtube.com/watch?v=JjKCy3H0A30
https://game.ci/docs/github/activation
https://game.ci/docs/github/builder