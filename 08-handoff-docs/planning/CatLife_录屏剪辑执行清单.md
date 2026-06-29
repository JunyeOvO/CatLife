# CatLife 录屏剪辑执行清单

日期：2026-06-29
目标：APK 或 Unity Game View 可运行后，用最少返工完成复赛演示视频素材采集和剪辑。

## 1. 录制前检查

| 检查 | 标准 |
|---|---|
| APK/Game View | 猫和小镇可见，状态链可跑 |
| 分辨率 | 竖版 1080x1920 或横版 1920x1080 |
| 通知 | 真机勿扰，隐藏个人通知 |
| 录音 | `adb screenrecord` 不录音，旁白后期配 |
| 密钥/账号 | 画面中不能出现 API Key、账号、聊天窗口 |
| 素材命名 | 按镜头编号保存 |

## 2. 建议录制命令

真机录制：

```powershell
adb shell screenrecord --bit-rate 8000000 --time-limit 180 /sdcard/catlife-demo.mp4
adb pull /sdcard/catlife-demo.mp4 "06-deliverables/final-submission/catlife-demo-device.mp4"
```

截图：

```powershell
adb exec-out screencap -p > "06-deliverables/final-submission/catlife-device-screenshot.png"
```

## 3. 分段素材清单

| 镜头 | 文件名 | 时长 | 必须出现 | 状态 |
|---|---|---:|---|---|
| 01 | `shot_01_town_overview.mp4` | 5-8s | 小镇全景 + CatLife 氛围 | 待录 |
| 02 | `shot_02_cat_idle.mp4` | 5s | 猫 idle/呼吸 | 待录 |
| 03 | `shot_03_state_normal.mp4` | 8s | 普通状态，猫活跃 | 待录 |
| 04 | `shot_04_state_transition.mp4` | 8s | 过渡动作 | 待录 |
| 05 | `shot_05_state_focus.mp4` | 10s | 专注状态，UI 降干扰 | 待录 |
| 06 | `shot_06_state_reward.mp4` | 8s | 奖励状态 | 待录 |
| 07 | `shot_07_apk_launch.mp4` | 5s | APK 启动证据 | 待录 |
| 08 | `shot_08_llm_feedback.mp4` | 6s | 大模型/本地反馈文案 | 待录 |
| 09 | `shot_09_evidence.mp4` | 5s | 安装/性能/日志证据 | 待录 |

## 4. 剪辑时间线

| 时间 | 内容 | 素材 |
|---:|---|---|
| 0-8s | 标题和小镇第一眼 | 01 |
| 8-20s | 痛点：分心到专注难 | UI 动效/旁白 |
| 20-35s | 主场景与猫 | 01 + 02 |
| 35-55s | 普通状态 | 03 |
| 55-75s | 过渡状态 | 04 |
| 75-105s | 专注状态 | 05 |
| 105-125s | 奖励状态 | 06 |
| 125-145s | AI 和隐私说明 | 08 |
| 145-165s | APK/工程证据 | 07 + 09 |
| 165-180s | 结尾三点亮点 | 海报/PPT |

## 5. 后期输出

| 项 | 标准 |
|---|---|
| 格式 | MP4 |
| 时长 | 尽量 <=3 分钟，最长 <=5 分钟 |
| 分辨率 | 1080x1920 或 1920x1080 |
| 字幕 | 简短，不遮挡猫和 UI |
| 音频 | 旁白清楚，背景音低音量 |
| 文件名 | `CatLife_作品演示视频_v1.mp4` |

## 6. 视频验收

- 第一屏 5 秒内看出作品名和视觉主体。
- 至少一次完整状态链：普通 -> 过渡 -> 专注 -> 奖励。
- 至少一次 APK 或真机证据。
- 大模型能力有实际输出或明确本地降级说明。
- 画面无个人隐私、密钥、无关窗口。
