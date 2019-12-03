SplitViewLib
---

Usage:

- Put SplitViewLib.dll into your unity project
- Ensure you enable it
![](https://i.imgur.com/BugWe5B.jpg)
- Using SplitLib
```c#
using SplitLib;
```

Example usage:
===

v0.2.0
----
- Create script like below.
```c#
using SplitLib;

public class TestScript : MonoBehaviour
{
    public GameObject bindCanvas;
    private SplitViewLib SplitLib;
    
    void Start()
    {
        SplitLib = new SplitViewLib();
        /**
         * void BindCamera(GameObject camera)
         * Bind camera element,
         * if this element is not camera, will add camera component on it
         */
        SplitLib.BindCamera(gameObject);
        splitViewLib.BindCanvas(bindCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPostRender()
    {
        /**
         * void UpdateOutputScreen()
         * Update the output to screen directly
         */
        SplitLib.UpdateOutputScreen(); // Ensure you put this method into OnPostRender()
    }
}

```
- Drag the script into your Camera/GameObject

v0.1.0
----

```c#
using SplitLib;
...

splitViewLib = new SplitViewLib();
...


Texture2D texture = splitViewLib.GetOutPutTextureFromCamera(camera);
...
```
and put the texture into anywhere you want to show on last screen.