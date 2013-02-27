xr-include
==========

An experimental text file pre-processor. 

Think of this as a slightly easier to type way of doing SSI (Server Side
Includes) and very simple variable expansion.

My main want for this was to construct a series of simple web pages that all
contained the same CSS and Javascripts, yet had some things (like headings and
titles) that would differ. 

I wanted to do the rest of the interface construction in Javascript and use RPC
after, so this should at least get you the simple server-side half.

Why would you want such a thing? Why not just use T4 templates? or Genshi? 
Well, if you have those at your disposal then they are proabably the right
thing. Use this if you don't have anything better :)

Example
========

Take some HTML templates "master.html":
```html
<html>
	<head>
		<title>$Title</title>
	</head>
	<body>
	
	<h1>$Heading</h1>
    <% xr-include src="$ContentTemplate" %>
	</body>
</html>
```
..and "content.html":
```html
<div>$Text</div>
```

You can render this by doing:
```csharp
var model = new MyModel() {
  Title = "Fallen Hero",
  Heading = "Gandalf", 
  ContentTemplate = "/content.html",
  Text = "Fly you fools!",
 }
var proc = new Processor();
proc.Context = model;
proc.Transform( "/master.html", Console.Out );
```
Which should render into:
```html
<html>
	<head>
		<title>Fallen Hero</title>
	</head>
	<body>
	
	<h1>Gandalf</h1>
    <div>Fly you fools!</div>
	</body>
</html>
```

BSD License
============

Copyright (c) 2013, Ian Norton
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this
list of conditions and the following disclaimer.  Redistributions in binary
form must reproduce the above copyright notice, this list of conditions and the
following disclaimer in the documentation and/or other materials provided
with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
