# 严辰乐 · Android集成+测试 · 复赛专业计划

## 你的核心价值
没有APK就没有"可运行产品"。你是把所有人的工作变成评委可以打开的东西的人。

## 提交目标
**可运行的产品包（APK或链接）**：评委能下载安装，打开看到猫，体验核心闭环。

---

## Week 1（5/26-6/1）：工程搭建

### Android工程
```
Android Studio Hedgehog 2023.1+
SDK: min 26, target 33
Project: CatLife
包名: com.catlife.app

app/src/main/java/com/catlife/app/
  MainActivity.java
  SplashActivity.java
  DataManager.java
```

### 数据存储接口
```java
public class DataManager {
    // 专注记录
    public void saveFocusSession(long durationSeconds, int growth) { }
    public int getTotalFocusMinutes() { }
    public List<String> getWeeklyRecords() { }

    // 设置
    public int getAdaptTime() { return 1; } // 分钟
    public String getCatName() { return "小橘"; }
}
```

### 空APK验证
- 写一个Hello World的MainActivity
- Build → APK
- 安装到手机上确认可以启动
- 这一步必须在W1完成，确保构建链路通

---

## Week 2（6/2-6/8）：Unity集成

### Unity → Android集成
方案：**Export Android Project**（最简单）

1. 陈泓森在Unity中：File → Build Settings → Android → Export
2. 你拿到导出的Android Project
3. 在Android Studio中打开
4. 编译运行

### 常见问题预案
| 问题 | 解决 |
|------|------|
| Gradle版本冲突 | 升级gradle wrapper |
| SDK版本不匹配 | 在build.gradle中调整compileSdk/targetSdk |
| 包名冲突 | 在Unity Player Settings中统一 |
| 64位要求 | 启用ARM64 |

### W2目标
- Unity场景在手机上成功运行
- 能看到猫和场景

---

## Week 3（6/9-6/15）：数据+行为采集+真机测试

### 行为数据采集（Android端）
```java
// 提供给陈泓森的数据接口
public class TouchDataProvider {
    private int touchCount = 0;
    private long lastTouchTime = 0;

    // Unity通过AndroidJavaClass调用
    public int getTouchRate() { return touchCount; }
    public long getIdleTime() {
        return System.currentTimeMillis() - lastTouchTime;
    }

    public void reset() { touchCount = 0; }
}
```

### 真机测试矩阵
| 机型 | Android版本 | 安装 | 启动 | Unity场景 | 内存 | 结论 |
|------|-----------|------|------|-----------|------|------|
| 机型1 | | | | | | |
| 机型2 | | | | | | |
| 机型3 | | | | | | |

### 极限测试
- 连续运行10分钟不崩溃
- 切后台再回前台
- 低电量模式
- 横竖屏切换

---

## Week 4（6/16-6/23）：APK打包+交付

### 最终APK
```bash
# 签名
./gradlew assembleRelease

# 检查
# 大小 < 150MB
# minSdk 26
# targetSdk 33
# 包含ARM64
```

### 性能标准
| 指标 | 目标 |
|------|------|
| 冷启动 | < 3秒 |
| 内存 | < 300MB |
| Unity帧率 | > 25fps |
| 崩溃率 | 0% |

### 回归测试清单
```
- [ ] 安装APK → 打开 → 开屏页正常
- [ ] 领养页 → 输入名称 → 进入主场景
- [ ] 猫在场景中显示
- [ ] 点击"开始专注" → 猫变安静 → UI隐藏
- [ ] 上滑 → 退出专注 → 恢复
- [ ] 查看专注记录
- [ ] 设置页修改适应时间
- [ ] 杀进程重启 → 数据保留
- [ ] 所有按钮有反馈（不卡死）
```

### 交付物
```
delivery/
  ├── CatLife_v1.0.apk        # 签名APK
  ├── README_安装说明.md       # 安装步骤+权限说明
  └── test_report.md           # 测试报告
```

---

## 与各成员对接点

| 对接人 | 对接内容 | 时间 |
|--------|---------|------|
| 陈泓森 | Unity导出工程 | W2初 |
| 吴若琪 | API密钥配置 | W2末 |
| 傅钧漪 | 录屏用APK | W3-W4 |

## 技术备忘
- SDK路径: `C:\Users\fujunye\AppData\Local\Android\Sdk`
- Gradle代理: `-Dhttp.proxyHost=172.24.48.1 -Dhttp.proxyPort=7897`
