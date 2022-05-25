# MonitorFileSystem

启动一个常驻服务，在文件系统发生更改时执行用户定义的操作，旨在实现灵活、高度自定义、可扩展的操作。

## State

该项目目前正在积极开发中，已经可以用来完成一些简单任务了。

## Feature

+ 提供 grpc 服务用于管理
+ 支持监听的文件系统事件(Created、Removed、Renamed 需要与Changed搭配使用):
  + Created
  + Removed
  + Renamed
  + Changed  
  其中，Changed 细分成以下事件
    + FileName
    + DirectoryName
    + Size
    + Attribute 
    + LastWrite
    + LastAccess
    + CreationTime
    + Security
+ 支持操作发生更改的文件或目录(~~删除线~~表示计划实现)
  + 移动到某路径
  + 解压缩文件
  + ~~删除~~
  + ~~作为参数调用外部脚本~~
+ 命令行管理工具
+ 支持外部配置文件(`.yaml`)，支持使用 `-d <path>` 选项临时指定配置文件

## Setup for Archlinux

现在已经在 Archlinux 中可用，使用 `yay -S monitorfs` 从 aur 安装，可以通过重复运行指令来安装到最新提交  
自带 `systemd` 服务，可以通过输入 `sudo systemctl enable --now MonitorFileSystem@root.service` 来立即启动并在重新启动后也自动启动的后台服务  
更改 `@` 后方的字符来指定其他用户  

## File System Events

### Created

单独启用该事件将不会执行动作，需要配合 `FileName` 或 `DirectoryName` 使用
