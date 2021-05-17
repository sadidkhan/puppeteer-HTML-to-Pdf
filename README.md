# Sample application to create PDF using Puppeteer

I had a plan to create an azure function that is responsible to generate pdf from html, I did not find any free library in .net core. Most of them are paid.

After a lot of googling, I came to know about a popular nodes js library [Puppeteer](https://github.com/puppeteer/puppeteer). That directs me to [Puppeteer Sharp](https://www.puppeteersharp.com/), a .NET port of the official Node.JS Puppeteer API. So, I used this library to create pdf.

For templating pupose, I used [Handlebars-Net](https://github.com/Handlebars-Net/Handlebars.Net) that is basically [Handlebars.js](https://handlebarsjs.com/guide/) templates in your .NET application. Handlebars.js is an extension to the Mustache templating language created by Chris Wanstrath. Handlebars.js and Mustache are both logicless templating languages that keep the view and the code separated like we all know they should be.


