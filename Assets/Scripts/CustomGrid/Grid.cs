using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Grid {

    private int width, height;
    private int[,] gridArray;

    public Grid(int w, int h) {
        width = w;
        height = h;
        gridArray = new int[w, h];
    }

}
