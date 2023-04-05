# **计算机动画大作业 - 基于Unity的元球建模实现**

基于Unity的屏幕空间后处理，使用RayMayching算法实现了Wyvill的六次多项式元球模型。

工程文件需要Unity2021.3.11f1c2或更高版本的Unity，使用Buildin渲染管线。

MetaballEditor.zip：可执行文件。要求windows系统（我用的win10），并支持dx11。随便解压到哪里，然后运行里面的MetaballProject.exe即可

- 使用UI添加元球以及修改参数
- 鼠标左键点选元球
- 鼠标右键按住可转动视角
- 鼠标中键按住可沿着X/Y/Z轴移动选中的元球（模仿Blender的操作）

# 元球算法实现

1.元球建模： Wyvill的六次多项式

2.元球渲染：RayMarching, 采用全局后处理进行绘制。

3.RayMarching的作用：

- 首先用SDF步进快速抵达有效半径处
- 然后使用Wyvill势场 + 定长步进找到渲染的表面。
- 具体渲染还是使用局部光照模型（实时）



**GlobalParas 全局参数 - Scriptable Object**

- BlobList - 所有元球的集合
- PositionList - 所有元球位置的集合
- rEffectList - 所有元球有效半径的集合
- densityList - 密度集合
- ColorList - 颜色集合
- Step - RayMarching在势场步进的最大不属（影响渲染距离）
- Accuracy - RayMarching步进步长（影响渲染精度和距离）



**Metaball**(using Wyvill's function) **元球的参数**:

- rEffect: 有效半径 - 可调参数
- density: 元球密度（负值表示减去） - 可调参数


  - threshold: 等势面阈值：在这个值的势场点被视作表面 - 可调参数


  - position: 位置 - 可随时间改变（被拖动）


  - mat: 材质，主要是颜色



脚本：

**Process - C#Script**：用于执行后处理渲染。

- SetParameters - 向shader传递参数
- StartPostProcess - 开启一个后处理

**CameraMove - C#Script**：用于控制相机移动 - 直接照搬Boid的相机移动

**SceneController 场景控制 - C#Script**：用于执行增加元球、修改参数等操作 - 照搬Boid的UI交互



Shader：RayMarchingShader

- vertex不需要做什么。
- frag:
  - Get Ray：inversed projection&view matrix
  - RayMarching
  - Blinn-Phong