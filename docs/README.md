# 2022 Winter XR Capstone Team 8 Website

The [team website](https://uwrealitylab.github.io/xrcapstone22wi-team8/) is built using the [Hugo](https://gohugo.io/) framework and the [ananke theme](https://github.com/theNewDynamic/gohugo-theme-ananke).

## Contents

- Quick description of the project
- Short bio/background of all team members
- Link to our Project Requirements Document (PRD)
- A hype/demo video
- Weekly updates on progress as project snippets

## Development Setup

Follow the [official instruction](https://gohugo.io/getting-started/installing) to install Hugo.

## Edit

### Add a New Post

1. Navigate to this current directory (`docs`) in the command line
2. Run `hugo new post-category/post-name.md`. For example, `hugo new snippets/week1.md`
3. You can find the new file as `content/en/post-category/post-name.md`
4. Edit the page (remember to fix the title, such as renaming from `week1` to `Week 1`)

### Edit an Existing Post

1. Open the markdown file in the `content` folder that you wish to modify
2. The metadata (title, image, description, etc.) is located at the top of the file
   - Change `draft: true` to `draft: false` when ready to publish
3. Use markdown syntax to write and format page contents
   - Don't know markdown? Refer to this [cheatsheet](https://enterprise.github.com/downloads/en/markdown-cheatsheet.pdf)
4. If you need to include images, put them under the `static/images` folder, and refer to them as `/images/file-name.png`

## Preview

1. Navigate to this current directory (`docs`) in the command line
2. Run `hugo server` to start the server
3. Open [http://localhost:1313/xrcapstone22wi-team8/](http://localhost:1313/xrcapstone22wi-team8/) in your browser to preview
4. Continue editing, save, and refresh the webpage to see the changes
5. Press <kbd>ctrl</kbd> + <kbd>c</kbd> to stop the server when done

## Deploy

The website is automatically deployed to GitHub Pages using the [official GitHub Actions setup](https://gohugo.io/hosting-and-deployment/hosting-on-github/) with some minor modifications for it to work in the `docs` folder.
