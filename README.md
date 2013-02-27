xr-include
==========

An experimental text file pre-processor. 

Think of this as a slightly easier to type way of doing 
SSI (Server Side Includes) and very simple variable expansion.

My main want for this was to construct a series of simple web pages that all contained the 
same CSS and Javascripts, yet had some things (like headings and titles) that would differ. 

I wanted to do the rest of the interface construction in Javascript and use RPC after, so 
this should at least get you the simple server-side half.

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
