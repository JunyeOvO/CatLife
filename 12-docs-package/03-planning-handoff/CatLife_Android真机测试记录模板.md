# CatLife Android 真机测试记录模板

日期：2026-06-29
用途：APK 构建完成后，按同一格式记录安装、启动、主流程、性能和证据文件。没有这张表，不应宣称 APK 已通过。

## 1. 测试环境

| 项 | 值 |
|---|---|
| 测试日期 | 待填 |
| 测试人 | 待填 |
| Unity 版本 | 待填 |
| APK 文件 | 待填 |
| APK SHA256 | 待填 |
| 设备型号 | 待填 |
| Android 版本 | 待填 |
| 测试方式 | 本地真机 / vivo 云真机 |
| 是否录屏 | 待填 |
| logcat 文件 | 待填 |

## 2. ADB 基础命令

```powershell
adb devices
adb install -r "06-deliverables/final-submission/CatLife_MVP_Android_v0.1.0.apk"
adb logcat -c
adb shell monkey -p com.catlife.mvp 1
adb logcat -d > "06-deliverables/final-submission/android-runtime-logcat.txt"
```

设备信息：

```powershell
adb shell getprop ro.product.model
adb shell getprop ro.build.version.release
adb shell dumpsys package com.catlife.mvp | Select-String version
```

## 3. 测试用例

| 编号 | 用例 | 步骤 | 通过标准 | 结果 | 证据 |
|---|---|---|---|---|---|
| T01 | 安装 | `adb install -r` | 返回 Success | 待填 | install log |
| T02 | 冷启动 | 点击图标或 monkey | 5 秒内进入首屏/主场景 | 待填 | 录屏 |
| T03 | 主场景 | 进入 mainscene | 猫和小镇可见 | 待填 | 截图 |
| T04 | 普通状态 | 等待默认状态 | 猫 idle/轻动作正常 | 待填 | 录屏 |
| T05 | 过渡状态 | 触发专注入口 | 猫切换到过渡动作 | 待填 | 录屏 |
| T06 | 专注状态 | 等待状态推进 | UI 降干扰，猫安静陪伴 | 待填 | 录屏 |
| T07 | 奖励状态 | 完成会话 | 出现奖励反馈 | 待填 | 录屏 |
| T08 | 退出流程 | 上滑/返回 | 可退出，无误触退出 | 待填 | 录屏 |
| T09 | 横竖屏 | 保持目标方向 | 构图不破坏 | 待填 | 录屏 |
| T10 | 稳定性 | 连续运行 3 分钟 | 无 crash/ANR | 待填 | logcat |

## 4. 性能记录

| 指标 | 值 | 证据 |
|---|---:|---|
| 冷启动耗时 | 待填 | 录屏计时 |
| 平均 FPS | 待填 | Profiler/Stats/云真机性能监控 |
| 峰值内存 | 待填 | Profiler/Android Studio/云真机性能监控 |
| Batches | 待填 | Unity Stats |
| Triangles | 待填 | Unity Stats |
| APK 大小 | 待填 | 文件属性 |
| 3 分钟运行结果 | 待填 | logcat |

## 5. 结论

| 项 | 结论 |
|---|---|
| 是否可用于录制演示视频 | 待填 |
| 是否可作为最终 APK 候选 | 待填 |
| 必须修复的问题 | 待填 |
| 可接受的已知限制 | 待填 |
