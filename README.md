# Devlog Entry - 11/15/24
## Introducing the Team:
Engine Co-Lead - Ethan Heffan
Engine Co-Lead - Arjun Krishnan
Design Co-Lead - Eric Wang
Tools Lead - Moore Macauley
Design Co-Lead - Igor Bessa
## Tools and materials
1. We’re planning on using Godot. Most of our engine experience comes from Unity, so we’re hoping to broaden our game dev horizons by learning an up and coming engine that isn’t too dissimilar from what we already know. While we could have used Unreal, Arjun and Ethan have some, albeit limited experience in Godot, so we figured it would be our best bet.
2. Godot has a variety of languages, but we’ll probably stick with C# because we’re all already familiar through Unity, so it’ll be the easiest way to transition to an entirely new engine. We might end up having to encounter C++, GDScript, or Godot’s Visual Scripting language depending on the requirements. For data processing we plan to use JSON files. For instance, we might use JSON to define the initial state of the grid or how it updates. 
3. The main tools we’ll use are VSCode for our IDE which’ll let us access GitHub for version control. For making 2D assets, we’ll probably use Photoshop and Aseprite and whatever relevant free assets we find online. If we need to make any 3D assets, we plan to use Blender, and we plan to use the equivalent of Unity’s scene view in Godot to create our levels.
4. Our alternate platform choices are to swap our code from C# into C++ . Since Godot is compatible with both of these programming languages, and we all have some experience with C++ through our CSE classes, we hope to have a relatively seamless swap between the two languages.
## Outlook
1. Our team is hoping to become more proficient at using Godot. We felt that with Unity’s recent actions only knowing Unity is probably not our best move, so we wanted to pick up a different engine. 
2. The hardest part will be learning how to use Godot since none of us have much experience. 
3. We are hoping to learn Godot and gain experience utilizing several Game Development Patterns to streamline the coordination process. We are also hoping to practice using GitHub in a group setting for collaboration. 

# Devlog Entry - 11/22/24
## How we satisfied the software requirements
The first step to starting the project was figuring out how to make the project in the first place. Godot has two versions: the regular and .NET versions, so we went with .NET that has C# support. Next, Godot has a native IDE that lacks useful features like code suggestions, so we figured out a way to link Godot to VSCode through several VSCode plugins. As part of that, we had to write a few .json files with one containing a directory to our local Godot.exe file, meaning we had to figure out how .gitignores work to ensure we didn't overwrite each others .json files.

Now for the project requirements itself... we created a player character that can move around a 5x5 grid with WASD. The player has an inventory of 3 seed types and can plant/harvest of the cell it's standing on. We accomplish this grid management with several layers of abstraction starting at a Grid Manager that handles requests from the player input and informs each cell when their info must be updated and re-rendered. The Grid Manager will contact a Grid class that holds an array of Cell structs containing each cell's water level, sun level, plant level, and plant type. The Grid is responsible for performing all operations and checks on each cell using each plant type's unique growth requirements. 

The game is simple. The player starts with a set number of seeds of all 3 types. They can plant seeds with the 1-3 keys respectively and harvest with Space. A plant can be harvested only when it reaches level 3 and each plant type has unique requirements for leveling up. The player must harvest 3 plants of each type to satisfy the win condition. When the player hits the PROGRESS TIME button, the Grid Manager tells the Grid to iterate through every cell and increases the water level randomly and chooses a random sun level. If a cell has a plant, it also checks whether the plant should level up based on its water/sun levels and its current neighboring cells. The GridManager then tells each cell's renderer to update to the current information.

## Reflection
Our team's plan didn't change too, too much from the original. The concept of a Design lead was interesting because we all had pretty much equal involvement with the design, especially considering most of the design was figured out already with the software requirements. It'll surely change as we get further into the project, so we'll see how the Design Lead role works later. As for the Tools and Engine roles, Moore and Ethan figured out a lot of foundation for how the project would be set up, while Arjun, Igor, and Eric implemented a lot of features on top of the existing foundation. Tools and Engine Lead didn't feel very appropriate for describing any of our duties, in fact our roles felt most similar to front-end and back-end developers. We imagine this division of labor will likely continue as the code foundation must be expanded and new features must be implemented. 

Our tools/engines have not changed at all really. We used C# in Godot with VSCode as the IDE and will still try to swap to C++ if it seems like it'll be a smooth transition.