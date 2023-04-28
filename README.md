# TreeGridView
一个以 TreeView 形式展示数据的 DataGridView

### 本控件对应如下需求：
希望以树形结构组织数据，但同时希望每一行数据都包含多个数据元素。

### 已实现功能：
1. 与 TreeView 的用法类似，以 Node 为单位组织数据。
2. 含有子 Node 的节点可以展开或折叠。
3. 每个 Node 前可以显示 CheckBox。
4. 可以为每个 Node 指定图标（通过 ImageList 或者直接指定I mage）。

### 未实现功能：
1. 绑定数据源。 
2. 虚模式（VirtualMode）。 
3. CheckBox 的未决状态（Mixed）。 

### Framework版本 
本工具需要.NET Framework 4.5.2 支持。

### 感谢以下项目的源码：
1、[AdvancedDataGridView](https://blogs.msdn.microsoft.com/markrideout/2006/01/08/customizing-the-datagridview-to-support-expandingcollapsing-ala-treegridview/) (MIT)
  
### 本控件源码基于 MIT 许可发布。
