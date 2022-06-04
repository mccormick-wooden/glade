# Environment Config

## Updating Script Template
The following introduces a new behavior script template with better default methods. The presence of these additional methods should hopefully help us more thoughtfully consider our behavior design.

1. Navigate to <UNITY-DIR>/Hub/Editor/2020.3.34f1/Editor/Data/Resources/ScriptTemplates
2. Copy the `81-C# Script-NewBehaviourScript.cs.txt` file to the directory

## Updating Default Line Endings
The following should be handled by `ProjectSettings/EditorSettings.asset`, but in case it is not:

1. In the Unity editor, navigate to `Editor > Project Settings > Editor`
2. Change the `Line Endings For New Scripts Mode` option to `Unix`
