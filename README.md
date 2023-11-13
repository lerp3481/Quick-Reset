# Quick Reset
An underwhelming "remaster" of my original DotHelper project.

"Quick Reset" is a reference to a very common phrase our marching director would say when he wanted us to get back to our previous formation.
I only intended this to be like an "internal project name" or something fancy like that, but afte realizing that I couldn't think of a better name, this name stuck.

So the story goes something like this:
======================================
In the summer of 2021, part of my intro-to-marching worksheet included midset calculations.
I spent more time developing a program to automate calculating the midset between two dots than it would've taken me if I worked it out by hand.
But that was the extent of my original intent really; just to make a midset calculator.

The assignment also included plotting the dots onto a marching field, which I didn't want to do myself for some reason?
I definitely wasted even more time writing code (originally in JavaScript in my DotHelper project) to draw a football field, and then draw a point where the dot was.

I was happy for a while since I was able to type in my own dots, and not only would I be able to find midsets, but I would also easily see where they are on the field.
Now the timeline of events is especially blurry, but the inconvenience of typing drill combined with procrastinating other assignments to work on this led to me developing a system where I scan text from full coordinate sheets so I can plot every dot for the entire band. 

The parts I specifically spending a lot of time coding was the playback system and the dot book maker.
I find the dot book maker infinitely more interesting to talk about since I remember entertaining myself with the idea that you could type in a performer label and push a "Make Dot Book" button, because those things are a pain to do manually.
The idea turned into a reality and it became the proudest thing I made.

Over time though, our drill writer did a bunch of things that seriously messed with my program and I always joked about having a back and forth battle with that guy.
Some things he did were entirely sensible and forced my code to handle edge cases, such as having drill go beyond the bounds of the field and needing to use multiple coordinate sheets for one performer because the text couldn't fit in one page.
However there were things he did which utterly baffled me, such as changing the standard step size from 8:5 to 10:5 and inventing new front-to-back references.

Somewhere along the way of fixing my code to deal with my drill writer's shenanigans, I realized how unwieldy it was to have the code for my project (which was getting pretty long) in one JavaScript file, so I basically translated everything to C# and split the code up nicely. 

So yeah, that's basically it.

Functionality:
==============
- Can read dots from coordinate sheet PDFs
- Can specify tempo changes for playback purposes
- Can make and open drill files, which are unique to this program
- Can visualize the band at every set, optionally displaying performer labels/drill numbers as well
- Can change the field dimensions
- Can output a video of the show
- Can output dot book pages

Gripes:
=======
- The way the coordinate sheets are formatted is hard coded in the program, which probably means this program can't open any coordinate sheet you're using
- The code is still a mess thanks to my naivety in good coding practices and documentation

Tour of the files:
==================
- DotBookMaker.cs: does all the calculations and drawings to make a dot book
- Drill.cs: a master class containing every performer and set definition. This is also where ranking performers based on drill difficulty takes place
- DrillFile.cs: saves and opens a file, unique to this program, representing the Drill class
- Field.cs: contains all the values needed for drawing the field, including step size and field dimensions
- FieldPanel.cs: draws the actual marching field to the screen using the values defined in Field.cs, as well as plotting dots
- FieldSettingsPanel.cs: the UI for changing the values defined in Field.cs
- Form1.cs: a master class containing all the sub-panel classes, as well as giving functionality to the File menu
- PdfParser.cs: where the coordinate sheet PDFs are scanned and filtered so that only relevant text is captured
- Performer.cs: contains a marcher's performer label and all their dots. This is also where left-to-rights and front-to-backs get converted from text to points and vice versa
- PerformerPanel.cs: the UI for displaying performer labels on screen and the "Make dot book" button
- PlaybackManager.cs: handles animating the dots on the field to make the show, and does all the timing calculatons
- Program.cs: contains the structure for holding tempo changes and when they happen
- ProgramPanel.cs: the UI for adding, removing or editing tempo changes and when they happen
- SetPanel.cs: the UI for navigating through the sets of the show, as well as the buttons for saving an image and exporting a video
- VideoExporter.cs: makes an MP4 of the show
