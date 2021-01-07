#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

void MapNullCheck_float(Texture2D map, out float4 Out) {
    if (map.r != 0) {
        Out = map.r;
    }
    Out = float4(0, 1, 0, 0);
}
#endif