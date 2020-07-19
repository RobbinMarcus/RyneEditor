@echo off
set /p levels="Enter voxel levels (octree depth, 7=low quality, 12=extremely high quality): "

CALL :Voxelize cube\cube.obj cube.bsvdag normals 

pause
EXIT /B 0

:Voxelize
echo Voxelizing %~1
CudaVoxelizer\CudaVoxelizer.exe "x64\Debug\Content\Models\%~1" "x64\Debug\Content\VoxelModels\%~2" %levels% %~3 %~4 %~5 %~6
EXIT /B 0