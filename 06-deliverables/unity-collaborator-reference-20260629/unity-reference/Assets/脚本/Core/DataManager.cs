using UnityEngine;
using System;

namespace CatLife.Core
{
    /// <summary>
    /// 用户数据（可序列化）
    /// </summary>
    [System.Serializable]
    public class UserData
    {
 public int coins;                    // 金币余额
        public int todayMinutes;             // 今日累计专注（按日期重置）
        public int totalFocusMinutes;        // 历史累计专注分钟数
        public int totalSessions;           // 总专注次数
        public int longestSessionMinutes;    // 最长专注时长（分钟）
        public int currentStreak;            // 连续专注天数
        public int lastFocusDay;             // 上次专注日期（YYYYMMDD）
        public float avgActivityScore;       // 平均活跃度分数
        public int catFullness;              // 猫咪饱食度 0~100
        public int catMood;                  // 猫咪心情 0~100

        // 用户设置
        public int focusDurationMinutes = 25;    // 专注时长（番茄钟）
        public bool soundEnabled = true;
        public bool vibrationEnabled = true;
        public bool autoFocusEnabled = true;     // 自动专注检测
        public int autoFocusThreshold = 30;      // 自动触发阈值（秒）

        // 统计
        public int totalCatInteractions;         // 与猫咪互动次数
        public int rewardCount;                  // 获得奖励次数

        public static UserData Default()
        {
            return new UserData
            {
                coins = 0,
                todayMinutes = 0,
                totalFocusMinutes = 0,
                totalSessions = 0,
                longestSessionMinutes = 0,
                currentStreak = 0,
                lastFocusDay = 0,
                avgActivityScore = 0.5f,
                catFullness = 100,
                catMood = 100,
                focusDurationMinutes = 25,
                soundEnabled = true,
                vibrationEnabled = true,
                autoFocusEnabled = true,
                autoFocusThreshold = 30,
                totalCatInteractions = 0,
                rewardCount = 0
            };
        }
    }

    /// <summary>
    /// 数据管理器
    /// 负责用户数据的持久化存储
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        private const string SAVE_KEY = "CatLife_UserData";
        private UserData _userData;

        public UserData UserData => _userData;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Load();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void Load()
        {
            string json = PlayerPrefs.GetString(SAVE_KEY, "");
            if (string.IsNullOrEmpty(json))
            {
                _userData = UserData.Default();
            }
            else
            {
                try
                {
                    _userData = JsonUtility.FromJson<UserData>(json);
                    Debug.Log($"[DataManager] 加载数据成功，累计专注 {_userData.totalFocusMinutes} 分钟");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[DataManager] 数据加载失败: {e.Message}");
                    _userData = UserData.Default();
                }
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(_userData);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
                Debug.Log("[DataManager] 数据已保存");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] 数据保存失败: {e.Message}");
            }
        }

        /// <summary>
        /// 记录一次专注完成
        /// </summary>
        public void RecordFocusSession(int minutes)
        {
            CheckAndResetDay();

            _userData.todayMinutes += minutes;
            _userData.totalFocusMinutes += minutes;
            _userData.totalSessions++;

            if (minutes > _userData.longestSessionMinutes)
            {
                _userData.longestSessionMinutes = minutes;
            }

            // 更新连续天数
            int today = GetTodayInt();
            int yesterday = today - 1;

            if (_userData.lastFocusDay == yesterday)
            {
                _userData.currentStreak++;
            }
            else if (_userData.lastFocusDay != today)
            {
                _userData.currentStreak = 1;
            }

            _userData.lastFocusDay = today;

            Save();
        }

        /// <summary>
        /// 新增金币
        /// </summary>
        public void AddCoins(int amount)
        {
            _userData.coins += amount;
            Save();
        }

        /// <summary>
        /// 消耗金币
        /// </summary>
        public bool SpendCoins(int amount)
        {
            if (_userData.coins < amount) return false;
            _userData.coins -= amount;
            Save();
            return true;
        }

        /// <summary>
        /// 喂猫（消耗金币，恢复饱食度）
        /// </summary>
        public bool FeedCat(int cost)
        {
            if (!SpendCoins(cost)) return false;
            _userData.catFullness = Mathf.Min(100, _userData.catFullness + 30);
            Save();
            return true;
        }

        /// <summary>
        /// 陪玩（消耗金币，提升心情）
        /// </summary>
        public bool PlayWithCat(int cost)
        {
            if (!SpendCoins(cost)) return false;
            _userData.catMood = Mathf.Min(100, _userData.catMood + 20);
            Save();
            return true;
        }

        /// <summary>
        /// 获取今日累计分钟数
        /// </summary>
        public int GetTodayMinutes()
        {
            CheckAndResetDay();
            return _userData.todayMinutes;
        }

        /// <summary>
        /// 按日期重置今日数据
        /// </summary>
        private void CheckAndResetDay()
        {
            int today = GetTodayInt();
            if (_userData.lastFocusDay != today)
            {
                _userData.todayMinutes = 0;
            }
        }

        /// <summary>
        /// 记录猫咪互动
        /// </summary>
        public void RecordCatInteraction()
        {
            _userData.totalCatInteractions++;
            Save();
        }

        /// <summary>
        /// 记录获得奖励
        /// </summary>
        public void RecordReward()
        {
            _userData.rewardCount++;
            Save();
        }

        /// <summary>
        /// 更新活跃度分数
        /// </summary>
        public void UpdateAvgActivityScore(float score)
        {
            // 指数移动平均
            _userData.avgActivityScore = _userData.avgActivityScore * 0.7f + score * 0.3f;
            Save();
        }

        /// <summary>
        /// 更新设置
        /// </summary>
        public void UpdateSettings(int focusMinutes, bool sound, bool vibration, bool autoFocus)
        {
            _userData.focusDurationMinutes = focusMinutes;
            _userData.soundEnabled = sound;
            _userData.vibrationEnabled = vibration;
            _userData.autoFocusEnabled = autoFocus;
            Save();
        }

        /// <summary>
        /// 获取今日日期整数（YYYYMMDD）
        /// </summary>
        private int GetTodayInt()
        {
            DateTime now = DateTime.Today;
            return now.Year * 10000 + now.Month * 100 + now.Day;
        }

        /// <summary>
        /// 重置所有数据
        /// </summary>
        public void ResetData()
        {
            _userData = UserData.Default();
            PlayerPrefs.DeleteKey(SAVE_KEY);
            Save();
            Debug.Log("[DataManager] 数据已重置");
        }
    }
}
