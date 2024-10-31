### 项目分析
1. Bezier 公式
2. Path point generater bezier
3. GUI curver line show
4. 还有 mesh 生成功能
    - mesh vertext + uv + noraml
    - indexes [0,8,1,1,8,9] [4,6,14,12,4,14,5,15,7,13,15,5]

### 实现分析
1. node 记录节点
2. 好的表现做简单，需要一些数据的支持
3. 做到功能提取分离
4. 去除编辑功能
5. 优化生成模式
6. bezier 曲线生成算法 | 求低阶导数算切线向量方向
    - 实现的功能很多，mesh 相关的也有做 normal 计算 ** 具体的实现方案查看对应的文档吧。正确的算法修正
    - display 计算也有
    - 这些部分的优先级不高，后面再看

### 拓展下个阶段
1. 物理骨骼效果
2. IK 效果