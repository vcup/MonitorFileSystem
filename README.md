# MonitorFileSystem

启动一个常驻服务，在文件系统发生更改时执行用户定义的操作，旨在实现灵活、高度自定义、可扩展的操作。

## State

该项目目前正在不积极开发中，已经可以用来完成一些简单任务了。

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
  + 作为参数调用外部脚本
+ 命令行管理工具
+ 支持外部配置文件(`.yaml`)，支持使用 `-d <path>` 选项临时指定配置文件

## Setup for Archlinux

现在已经在 Archlinux 中可用，使用 `yay -S monitorfs` 从 aur 安装，可以通过重复运行指令来安装到最新提交  
自带 `systemd` 服务，可以通过输入 `sudo systemctl enable --now MonitorFileSystem@root.service` 来立即启动并在重新启动后也自动启动的后台服务  
更改 `@` 后方的字符来指定其他用户  

## File System Events

### Created

在文件系统中有新实体创建时引发，单独启用该事件将不会执行动作，需要配合 `FileName` 或 `DirectoryName` 使用  
当设置了 `FileName` 则在有新文件被创建时引发事件，如果设置了 `DirectoryName` 则是在有新文件夹被创建时引发，两者可同时设置  
注意，从监视路径之外的地方移动文件或文件夹到监视路径内会被视作新创建文件或文件夹

### Removed

在文件系统中有文件或文件夹被删除时引发，单独启用该事件无效果，需配合 `FileName` 或 `DirectoryName` 使用  
设置了 `FileName` 则在有文件被删除时引发事件，如果设置了 `DirectoryName` 则是有文件夹被删除时触发，两者可同时设置  
注意，将文件或文件夹移出监视路径会被视作删除

### Renamed

在文件系统中有文件或文件夹被重命名时触发，单独启用该事件无效果，需配合 `FileName` 或 `DorectpryName` 使用  
设置了 `FileName` 则只监视文件的重命名，设置 `Directory` 则只监视文件夹的重命名，两者可同时设置  
在监视路径内移动文件或文件夹时更改名称会被视作重命名

### Changed

会同时设置以下所有 `FileName` `DirectoryName` `Size` `Attribute` `LastWrite` `LastAccess` `CreationTime` `Security` 事件  
同时设置上述事件也等价于设置了 `Changed`

## Operates

### Move

触发时将观察到变更的文件或文件夹移动到指定路径  
其中:
+ 指定路径不存在且变更的是文件时，将文件移动到指定路径并重新指派名称
+ 指定路径不存在且变更的是文件夹时，将文件夹移动到目标位置并重新指派名称
+ 指定路径存在且是文件夹，将发生变更的文件或文件夹移动到指定文件夹
+ 指定路径存在且是文件，且变更的是文件时覆盖指定文件
+ 指定路径存在且是文件，但变更的是文件夹时，我猜会报错

### Unpack

目前只支持解压 `.zip` 格式的压缩文件，当变更的时文件夹时会忽略该动作  
具有类似 BandiZip 的 SmartExtract 功能的行为  
会先将压缩包解压到临时文件夹再移动到目标位置  
在Windows系统中这个位置是 `%userprofile%\AppData\Local\Temp`  
在Linux系统中则通常在 `/tmp`，注意，这需要 `tmpfs` 的容量大于压缩包解压后体积

### Command

该操作不支持 MAC 系统，在Windows上会调用 `cmd.exe` 来执行命令，在Linux则调用 `/usr/bin/sh`  
使用一个字符串作为命令模板，例如 `echo {0}`。其中 `{0}` 作为占位符，需要另外设置实际传入的参数格式化成最终指令  
支持的参数有 `Name` `Path` `OldName` `OldPath`  
其中:
+ `Name` 是发生变更的文件或文件夹的名称
+ `Path` 是发生变更的文件或文件夹的路径
+ `OldName` 是发生 `Renamed` 事件时，文件或文件夹的重命名前的名称
+ `OldPath` 是发生 `Renamed` 事件时，文件或文件夹的重命名前的路径
