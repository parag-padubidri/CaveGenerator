Procedural Cave Generator
-------------------------
Tool to generate Cellular Automata based caves procedurally using primitives

Description:
------------
The tool enables a custom "Cave Generator" menu to launch an editor window that lets the user generate procedural caves based on the cellular automata model by setting various parameters. 

Usage Description:
------------------
1) Download the ProceduralCaveGenerator unity package file from the repository
2) Create a new project in unity & import the downloaded package
3) Select Cave Generator from the top Menu Bar & select Launch Editor
4) Enter values for CaveMap width & height eg: 50 for both
5) Enter percentage of walls required (ideal is between 20-30) eg: 27
6) Enter threshold value for empty cells to be generated (ideal is upto 1/3 of width * height) eg: 800 for 50 * 50. Threshold value autoupdates depening on width & height entered to generate reasonable cave maps.
7) Enter seed value as 0 to keep generating randomized caves or a value other than 0 to be able to regenerate a cave eg: 47. If seed value is set it will ignore threshold value.
8) Click Generate Cave button to generate randomized cave or regenerate previously generated cave from seed. If the seed is not set i.e. set to "0", the user can keep on clicking Generate Cave button to get different randomized caves. 
9) Click on Save Generated Cave as Prefab button to save the currently generated cave as a prefab with current timestamp. This button is not enabled unless a new cave is generated or if the user clicks on play button to run the generated cave in the scene.
10) Click on Remove Single Walls button to remove single walls if any from the currently generated cave. This button is not enabled unless a new cave is generated or if the user clicks on play button to run the generated cave in the scene.
11) You can test the cave by dropping the FPSController and MiniMap prefabs in the generated cave from the FPSController Prefabs folder in the package & pressing play.

Alternatively, you can download the entire project from the repository & play around with the demo GameScene that has been setup.

References:
-----------
Cellular Automata Rules - http://www.roguebasin.com/index.php?title=Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels#Rule_Tweaking
Flood Fill Algorithm - https://en.wikipedia.org/wiki/Flood_fill
