# Assignment Write-Ups

Use this directory to store the raw (markdown) versions of the assignment write ups.

## Generating PDFs

Tested on M1 Mac.

---

### Prerequisites

[Install brew](https://brew.sh) if needed

```
brew install pandoc homebrew/cask/basictex
```

### Generate a single .pdf

```
pandoc example.md -s -o example.pdf --pdf-engine=/Library/TeX/texbin/pdflatex
```

The `pdf-engine` argument may not be required for intel-based Macs.

### Compile all .md into .pdf

```
ls *.md | \
grep -v 'README.md' | \
sed 's/.md$//' | \
xargs -L 1 -I {} \
pandoc {}.md -s -o {}.pdf --pdf-engine=/Library/TeX/texbin/pdflatex
```
