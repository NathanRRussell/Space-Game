using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGeneration : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] int minHeight, maxHeight;
    [SerializeField] int repeatNum;
    [SerializeField] GameObject lunarSoil, lunarCrust;

    void Start() {
        Generation();
    }

    void Generation() {
        int repeatValue = 0;
        for(int x=0;x<width;x++) {
            if(repeatValue == 0) {
                height = Random.Range(minHeight, maxHeight);
                GenerateFlatPlatform(x);
                repeatValue = repeatNum;
            } else {
              GenerateFlatPlatform(x);
              repeatValue--;  
            }
        }
    }

    void GenerateFlatPlatform(int x) {
        for(int y=0;y<height;y++) {
            spawnObject(lunarSoil, x, y);
        }
        spawnObject(lunarCrust, x, height);
    }

    void spawnObject(GameObject obj, int width, int height) {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
}
