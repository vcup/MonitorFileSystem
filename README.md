# MonitorFileSystem

启动一个常驻服务，在文件系统发生更改时执行用户定义的操作，旨在实现灵活、高度自定义、可扩展的操作。

## State

该项目目前正在积极开发中，尚处于不可用状态。

## Feature

+ 提供 grpc 服务用于管理
+ 支持监听的文件系统事件(但服务从未工作过，所以具体列表有待调整，这里只是列出微软文档表示支持的事件):
  + Created
  + Removed
  + Renamed
  + FileName
  + DirectoryName
  + Size
  + Attribute 
  + LastWrited
  + LastAccess
  + CreationTime
  + Security
+ 支持操作发生更改的文件或目录(~~删除线~~表示计划实现)
  + 移动到某路径
  + 解压缩文件
  + ~~删除~~
  + ~~作为参数调用外部脚本~~
+ 命令行管理工具(计划实现)
