# Oni2Xml

Converts Oxygen Not Included game saves to human readable and editable ~~xml~~ json.

# DEPRECATED

This repository is no longer mantained.  Version 1.1.0 will work up to the Rancher update.

It is being replaced by [oni-save-parser](https://github.com/RoboPhred/oni-save-parser), and will eventually gain a user interface.

## Compatibility:

This tool was tested as successfully loading Thermal update and Rancher update saves.  It has only been tested saving Thermal saves.

Note that it will not re-compress your save, so the resulting save file will be much larger.  The game should load this uncompressed save correctly and compress it next time you make a new save.

While the tool was developed and works with pre-TU saves, it is currently set up to act on Thermal Update saves only.
As far as I can tell, there is no difference in format.  The check is done by inspecting the save file's version header.


Get it here: https://github.com/RoboPhred/Oni2Xml/releases


```
oni2xml export-objects <save file path>
```
Exports save files to <save-name>.gameObjects.json

```
oni2xml import-objects <game object json path> <save path>
```
Replaces the save file's game objects with ones defined in the gameObjects.json file


Game objects are stored by object tag / name:
```
{
  "tag": "TheObject",
  "gameObjects": [...]
 }
```
Game objects keys of interest:
position
rotation
scale: This one functions fine, can change the size of dups and so on
folder: The id of the unity editor folder this game object is stored in.  Do not touch.
components: An array of MonoBehavior and their serialized data.

Game object components are stored with the following keys
name: MonoBehaviorName
fields: object of typed json values for the auto-serialized fields
properties: object of typed json values for the auto-serialized properties
saveLoadableDetailsData: Dont touch this, it contains manually serialized data for the behavior.  Eventually I will get around to creating serializers for these.


Things to try:

Editing dup traits and stats:
search for
```
"tag": "Minion"
```

Traits are under a component called "Klei.AI.Traits".
Add or remove string values to the TraitIds.values array, in this format:
```
{
  "$type": "Oni2Xml.TypeData.PrimitiveInstanceData, Oni2Xml",
  "value": "DiversLung"
},
```


Attributes are stored under a component called "Klei.AI.AttributeLevels"
Attributes are stored in a field array called saveLoadLevels
Each attribute is object data containing 3 fields:
attributeId: name of attribute
experience: current experience at attribute
level: current level at attribute

Search for the attribute name you want, it will look like:
```
"attributeId": {
  "$type": "Oni2Xml.TypeData.PrimitiveInstanceData, Oni2Xml",
  "value": "Construction"
},
"experience": {
  "$type": "Oni2Xml.TypeData.PrimitiveInstanceData, Oni2Xml",
  "value": 106.653061
},
"level": {
  "$type": "Oni2Xml.TypeData.PrimitiveInstanceData, Oni2Xml",
  "value": 1
}
```



More details to follow.

TODO:
Code needs to be cleaned up, set up for multiple versions, export other sections.
UI - Template files that provide ui for editing sections and so on.  Use case: make it easy to spawn in new dups, npcs, buildings, so on.


Licenseish:
Don't use it in other projects at the moment.  Will revisit this later when I am further along.

Contributing:
Go for it.  fork on github, add stuff, send pull request.
