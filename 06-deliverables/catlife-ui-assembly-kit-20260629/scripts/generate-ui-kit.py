from __future__ import annotations

import csv
import hashlib
import json
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont, ImageFilter


REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "06-deliverables" / "catlife-ui-assembly-kit-20260629"
PREVIEWS = KIT / "assets" / "previews"
ICONS = KIT / "assets" / "icons"
TEXTURES = KIT / "assets" / "textures"
LAYOUT = KIT / "layout"

W, H = 1080, 1920

COLORS = {
    "cream": (255, 244, 226),
    "cream2": (255, 237, 207),
    "orange": (239, 142, 66),
    "orange_dark": (176, 96, 40),
    "green": (91, 160, 93),
    "green_dark": (56, 120, 72),
    "teal": (79, 165, 157),
    "sky": (157, 204, 232),
    "brown": (118, 77, 48),
    "dark": (68, 52, 42),
    "muted": (136, 112, 92),
    "white": (255, 255, 255),
    "line": (234, 214, 186),
    "gold": (242, 194, 82),
}


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont:
    candidates = [
        r"C:\Windows\Fonts\msyhbd.ttc" if bold else r"C:\Windows\Fonts\msyh.ttc",
        r"C:\Windows\Fonts\simhei.ttf",
        r"C:\Windows\Fonts\arial.ttf",
    ]
    for item in candidates:
        p = Path(item)
        if p.exists():
            return ImageFont.truetype(str(p), size=size)
    return ImageFont.load_default()


F = {
    "title": font(74, True),
    "h1": font(52, True),
    "h2": font(38, True),
    "body": font(30),
    "body_b": font(30, True),
    "small": font(24),
    "tiny": font(20),
    "num": font(64, True),
}


def rounded(draw: ImageDraw.ImageDraw, xy, r, fill, outline=None, width=1):
    draw.rounded_rectangle(xy, radius=r, fill=fill, outline=outline, width=width)


def text(draw, xy, value, fill=COLORS["dark"], fnt=None, anchor=None):
    draw.text(xy, value, fill=fill, font=fnt or F["body"], anchor=anchor)


def fit_cover(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    iw, ih = img.size
    tw, th = size
    scale = max(tw / iw, th / ih)
    nw, nh = int(iw * scale), int(ih * scale)
    img = img.resize((nw, nh), Image.Resampling.LANCZOS)
    left = (nw - tw) // 2
    top = (nh - th) // 2
    return img.crop((left, top, left + tw, top + th))


def source_image(name: str) -> Image.Image | None:
    paths = {
        "scene_only": KIT / "assets" / "camera-reference" / "generated_scene_only_cat_town.png",
        "town": KIT / "assets" / "camera-reference" / "town_home_overview_top.png",
        "town_front": KIT / "assets" / "camera-reference" / "town_home_overview_front.png",
        "town_reward": KIT / "assets" / "camera-reference" / "town_reward_tree_close.png",
        "focus_outdoor": KIT / "assets" / "camera-reference" / "focus_ui_outdoor_reference.png",
        "splash_phone": KIT / "assets" / "camera-reference" / "catlife_splash_phone.png",
        "cat": REPO / "06-deliverables" / "demo-preview-20260629" / "assets" / "unity-cat-gameview.png",
        "idle": REPO / "06-deliverables" / "demo-preview-20260629" / "assets" / "cat-idlebreath-contact-sheet.png",
        "happy": REPO / "06-deliverables" / "demo-preview-20260629" / "assets" / "cat-tailwaghappy-contact-sheet.png",
    }
    p = paths[name]
    if p.exists():
        return Image.open(p).convert("RGB")
    return None


def phone_base() -> Image.Image:
    img = Image.new("RGB", (W, H), COLORS["cream"])
    d = ImageDraw.Draw(img)
    d.rectangle((0, 0, W, H), fill=(255, 247, 235))
    for y in range(0, H, 12):
        c = int(250 - min(y / H * 18, 18))
        d.line((0, y, W, y), fill=(255, c, 220), width=1)
    return img


def scene_only_background() -> Image.Image:
    scene = source_image("scene_only") or source_image("town_front") or source_image("town")
    if scene:
        return fit_cover(scene, (W, H))
    return town_background().resize((W, H), Image.Resampling.LANCZOS)


def town_background() -> Image.Image:
    town = source_image("town_front") or source_image("town")
    if town:
        bg = fit_cover(town, (W, 1180)).filter(ImageFilter.GaussianBlur(0.3))
    else:
        bg = Image.new("RGB", (W, 1180), COLORS["green"])
        d = ImageDraw.Draw(bg)
        d.ellipse((120, 120, 980, 760), fill=(112, 177, 93))
    overlay = Image.new("RGBA", bg.size, (255, 248, 232, 58))
    bg = Image.alpha_composite(bg.convert("RGBA"), overlay).convert("RGB")
    return bg


def top_status(draw, title="CatLife", subtitle="今天已安静陪伴 18 分钟"):
    text(draw, (64, 70), title, COLORS["dark"], F["h1"])
    text(draw, (64, 132), subtitle, COLORS["muted"], F["small"])
    rounded(draw, (884, 70, 1016, 134), 28, (255, 255, 255), COLORS["line"])
    text(draw, (950, 102), "设置", COLORS["brown"], F["small"], "mm")


def bottom_nav(draw, active="home"):
    rounded(draw, (48, 1690, 1032, 1838), 46, (255, 255, 255), COLORS["line"], 2)
    items = [("home", "小镇"), ("focus", "专注"), ("record", "记录"), ("cat", "猫咪")]
    x0 = 150
    for i, (key, label) in enumerate(items):
        x = x0 + i * 260
        color = COLORS["orange"] if key == active else COLORS["muted"]
        draw.ellipse((x - 22, 1722, x + 22, 1766), fill=color)
        text(draw, (x, 1800), label, color, F["small"], "mm")


def cat_badge(draw, xy, mood="安静陪伴"):
    x, y = xy
    rounded(draw, (x, y, x + 230, y + 72), 32, (255, 255, 255), COLORS["line"], 2)
    draw.ellipse((x + 18, y + 16, x + 56, y + 54), fill=COLORS["orange"])
    text(draw, (x + 78, y + 36), mood, COLORS["dark"], F["small"], "lm")


def cat_speech_bubble(draw, xy, message):
    x1, y1, x2, y2 = xy
    rounded(draw, (x1, y1, x2, y2), 34, (255, 255, 255, 232), (255, 220, 156), 3)
    tail_x = int((x1 + x2) * 0.5)
    draw.polygon(
        [(tail_x - 24, y2 - 2), (tail_x + 34, y2 - 2), (tail_x + 4, y2 + 42)],
        fill=(255, 255, 255, 232),
        outline=(255, 220, 156),
    )
    text(draw, ((x1 + x2) // 2, (y1 + y2) // 2), message, COLORS["brown"], F["body_b"], "mm")


def screen_splash():
    img = scene_only_background()
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle((0, 0, W, H), fill=(30, 24, 18, 58))
    draw = ImageDraw.Draw(img)
    text(draw, (84, 120), "CatLife", COLORS["white"], F["title"])
    text(draw, (88, 208), "让专注自然发生", (255, 242, 218), F["body_b"])
    rounded(draw, (270, 1586, 810, 1692), 52, COLORS["orange"], None)
    text(draw, (540, 1638), "进入猫咪小镇", COLORS["white"], F["body_b"], "mm")
    text(draw, (540, 1818), "本地识别 · 温和陪伴 · 可选智能解释", (255, 242, 218), F["small"], "mm")
    return img


def screen_main():
    img = scene_only_background()
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle((0, 0, W, 320), fill=(30, 24, 18, 55))
    d.rectangle((0, 1480, W, H), fill=(30, 24, 18, 42))
    draw = ImageDraw.Draw(img)
    text(draw, (64, 72), "CatLife", COLORS["white"], F["h1"])
    text(draw, (64, 136), "今天已安静陪伴 18 分钟", (255, 244, 224), F["small"])
    for i, label in enumerate(["猫咪", "记录", "设置"]):
        y = 430 + i * 112
        draw.ellipse((930, y, 1012, y + 82), fill=(255, 255, 255), outline=COLORS["line"], width=2)
        text(draw, (971, y + 42), label[:1], COLORS["brown"], F["small"], "mm")
    camera_buttons = [
        ("左旋", 62, 1118),
        ("右旋", 182, 1118),
        ("前进", 62, 1238),
        ("后退", 182, 1238),
    ]
    for label, x, y in camera_buttons:
        draw.ellipse((x, y, x + 96, y + 96), fill=(255, 255, 255), outline=COLORS["line"], width=2)
        text(draw, (x + 48, y + 48), label, COLORS["brown"], F["tiny"], "mm")
    rounded(draw, (64, 1540, 1016, 1658), 56, COLORS["orange"], None)
    text(draw, (540, 1599), "开始专注", COLORS["white"], F["h2"], "mm")
    rounded(draw, (86, 1698, 358, 1776), 38, (255, 255, 255), COLORS["line"], 2)
    text(draw, (222, 1737), "普通状态", COLORS["brown"], F["small"], "mm")
    cat_speech_bubble(draw, (230, 830, 850, 934), "先不用急，我在这里。")
    return img


def screen_focus_setup():
    img = scene_only_background().filter(ImageFilter.GaussianBlur(0.4))
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle((0, 0, W, H), fill=(35, 28, 22, 62))
    draw = ImageDraw.Draw(img)
    text(draw, (64, 94), "准备进入专注", COLORS["white"], F["h1"])
    text(draw, (68, 160), "我会把动作放慢，陪你进入这段时间。", (255, 244, 224), F["small"])
    labels = [("15 分钟", 96), ("25 分钟", 386), ("45 分钟", 676)]
    for label, x in labels:
        fill = COLORS["orange"] if label.startswith("25") else COLORS["white"]
        fg = COLORS["white"] if label.startswith("25") else COLORS["dark"]
        rounded(draw, (x, 1440, x + 224, 1538), 42, fill, COLORS["line"], 2)
        text(draw, (x + 112, 1489), label, fg, F["body_b"], "mm")
    rounded(draw, (96, 1590, 984, 1702), 54, COLORS["teal"], None)
    text(draw, (540, 1646), "开始", COLORS["white"], F["h2"], "mm")
    text(draw, (540, 1788), "安静陪伴 · 智能解释关闭", COLORS["white"], F["small"], "mm")
    return img


def screen_focus_running():
    img = scene_only_background()
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle((0, 0, W, H), fill=(20, 75, 72, 72))
    draw = ImageDraw.Draw(img)
    text(draw, (540, 124), "专注进行中", COLORS["white"], F["h1"], "mm")
    text(draw, (540, 224), "18:42", COLORS["white"], F["num"], "mm")
    rounded(draw, (302, 306, 778, 374), 34, (255, 255, 255), None)
    d.rectangle((322, 324, 552, 356), fill=COLORS["green"])
    text(draw, (540, 338), "50%", COLORS["dark"], F["small"], "mm")
    cat_speech_bubble(draw, (210, 760, 870, 864), "我会轻轻陪着你，不打扰。")
    text(draw, (540, 1750), "上滑退出", (235, 250, 247), F["body"], "mm")
    return img


def screen_reward():
    img = scene_only_background()
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle((0, 0, W, H), fill=(55, 32, 18, 68))
    draw = ImageDraw.Draw(img)
    text(draw, (540, 110), "完成了一段安静时间", COLORS["white"], F["h1"], "mm")
    draw.ellipse((446, 214, 634, 402), fill=COLORS["gold"], outline=COLORS["orange_dark"], width=6)
    text(draw, (540, 308), "爪印 +1", COLORS["brown"], F["body_b"], "mm")
    cat_speech_bubble(draw, (162, 720, 918, 824), "完成啦，猫咪给你一个小爪印。")
    rounded(draw, (104, 1508, 976, 1618), 48, COLORS["orange"], None)
    text(draw, (540, 1563), "回到小镇", COLORS["white"], F["h2"], "mm")
    text(draw, (540, 1696), "25 分钟 · 稳定度 82 · 中断 1 次", COLORS["white"], F["body_b"], "mm")
    return img


def screen_records():
    img = phone_base()
    draw = ImageDraw.Draw(img)
    top_status(draw, "记录", "不用打卡压力，只看安静时间")
    rounded(draw, (64, 240, 1016, 492), 44, COLORS["white"], COLORS["line"], 2)
    text(draw, (116, 302), "今日", COLORS["muted"], F["small"])
    text(draw, (116, 390), "48 分钟", COLORS["dark"], F["num"])
    text(draw, (580, 390), "2 段完成", COLORS["brown"], F["h2"], "lm")
    chart_y = 710
    days = [22, 38, 16, 45, 31, 60, 48]
    for i, v in enumerate(days):
        x = 106 + i * 128
        h = int(v * 5)
        rounded(draw, (x, chart_y + 320 - h, x + 70, chart_y + 320), 22, COLORS["teal"], None)
        text(draw, (x + 35, chart_y + 364), f"D{i+1}", COLORS["muted"], F["tiny"], "mm")
    text(draw, (64, 630), "最近 7 天", COLORS["dark"], F["h2"])
    rounded(draw, (64, 1180, 1016, 1390), 38, COLORS["white"], COLORS["line"], 2)
    text(draw, (112, 1245), "猫咪洞察", COLORS["muted"], F["small"])
    text(draw, (112, 1310), "你在晚上更容易进入稳定状态。", COLORS["dark"], F["body_b"])
    bottom_nav(draw, "record")
    return img


def screen_privacy():
    img = phone_base()
    draw = ImageDraw.Draw(img)
    top_status(draw, "隐私与智能解释", "只上传聚合特征，默认本地优先")
    cards = [
        ("本地行为识别", "开启 · 只统计本 App 内点击、滑动、停留"),
        ("智能解释", "关闭 · 开启后只发送聚合分值"),
        ("不会采集", "不录屏、不读取输入内容、不跨 App 监控"),
        ("清除记录", "删除本地会话摘要"),
    ]
    y = 260
    for title, desc in cards:
        rounded(draw, (64, y, 1016, y + 200), 38, COLORS["white"], COLORS["line"], 2)
        text(draw, (116, y + 62), title, COLORS["dark"], F["body_b"])
        text(draw, (116, y + 126), desc, COLORS["muted"], F["small"])
        y += 238
    rounded(draw, (64, 1450, 1016, 1580), 40, COLORS["teal"], None)
    text(draw, (540, 1515), "查看大模型调用说明", COLORS["white"], F["body_b"], "mm")
    return img


def icon(name: str, color):
    img = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    if name == "paw":
        for x, y in [(72, 72), (124, 54), (176, 72), (98, 122), (158, 122)]:
            d.ellipse((x - 24, y - 24, x + 24, y + 24), fill=color)
        d.ellipse((66, 132, 190, 222), fill=color)
    elif name == "clock":
        d.ellipse((38, 38, 218, 218), outline=color, width=18)
        d.line((128, 128, 128, 72), fill=color, width=16)
        d.line((128, 128, 172, 148), fill=color, width=16)
    elif name == "leaf":
        d.ellipse((48, 48, 210, 178), fill=color)
        d.line((72, 188, 190, 70), fill=(255, 255, 255, 180), width=12)
    elif name == "moon":
        d.ellipse((52, 38, 210, 220), fill=color)
        d.ellipse((106, 20, 238, 178), fill=(0, 0, 0, 0))
    elif name == "settings":
        d.ellipse((84, 84, 172, 172), outline=color, width=18)
        for a in range(0, 360, 45):
            x = 128 + math.cos(math.radians(a)) * 82
            y = 128 + math.sin(math.radians(a)) * 82
            d.line((128, 128, x, y), fill=color, width=12)
    return img


def save_assets():
    PREVIEWS.mkdir(parents=True, exist_ok=True)
    ICONS.mkdir(parents=True, exist_ok=True)
    TEXTURES.mkdir(parents=True, exist_ok=True)
    LAYOUT.mkdir(parents=True, exist_ok=True)

    screens = {
        "01_splash.png": screen_splash(),
        "02_main_town.png": screen_main(),
        "03_focus_setup.png": screen_focus_setup(),
        "04_focus_running.png": screen_focus_running(),
        "05_reward_summary.png": screen_reward(),
        "06_records.png": screen_records(),
        "07_privacy_llm.png": screen_privacy(),
    }
    for name, img in screens.items():
        img.save(PREVIEWS / name)

    thumb_w, thumb_h = 270, 480
    sheet = Image.new("RGB", (thumb_w * 4, thumb_h * 2 + 86), COLORS["cream"])
    sd = ImageDraw.Draw(sheet)
    sd.text((32, 22), "CatLife UI Preview Contact Sheet", fill=COLORS["dark"], font=F["body_b"])
    for idx, (name, img) in enumerate(screens.items()):
        x = (idx % 4) * thumb_w
        y = 86 + (idx // 4) * thumb_h
        sheet.paste(img.resize((thumb_w, thumb_h), Image.Resampling.LANCZOS), (x, y))
        sd.rectangle((x, y, x + thumb_w - 1, y + thumb_h - 1), outline=COLORS["line"], width=2)
        sd.text((x + 12, y + 12), name.replace(".png", ""), fill=COLORS["white"], font=F["tiny"])
    sheet.save(PREVIEWS / "00_contact_sheet.png")

    for name, color in {
        "icon_paw.png": COLORS["orange"],
        "icon_clock.png": COLORS["teal"],
        "icon_leaf.png": COLORS["green"],
        "icon_moon.png": (82, 111, 154),
        "icon_settings.png": COLORS["brown"],
    }.items():
        icon(name.replace("icon_", "").replace(".png", ""), color).save(ICONS / name)

    tile = Image.new("RGB", (512, 512), COLORS["cream"])
    td = ImageDraw.Draw(tile)
    for y in range(0, 512, 64):
        td.line((0, y, 512, y), fill=COLORS["cream2"], width=3)
    tile.save(TEXTURES / "soft_paper_panel.png")

    button = Image.new("RGBA", (512, 180), (0, 0, 0, 0))
    bd = ImageDraw.Draw(button)
    bd.rounded_rectangle((0, 0, 512, 180), radius=70, fill=COLORS["orange"])
    button.save(TEXTURES / "button_primary_orange.png")

    layout = {
        "canvas": {"referenceResolution": [1080, 1920], "match": 0.5, "orientation": "portrait"},
        "screens": [
            {"id": "Splash", "preview": "assets/previews/01_splash.png", "unityPanel": "SplashPanel"},
            {"id": "MainTown", "preview": "assets/previews/02_main_town.png", "unityPanel": "MainTownPanel"},
            {"id": "FocusSetup", "preview": "assets/previews/03_focus_setup.png", "unityPanel": "FocusSetupPanel"},
            {"id": "FocusRunning", "preview": "assets/previews/04_focus_running.png", "unityPanel": "FocusOverlay"},
            {"id": "RewardSummary", "preview": "assets/previews/05_reward_summary.png", "unityPanel": "RewardPanel"},
            {"id": "Records", "preview": "assets/previews/06_records.png", "unityPanel": "RecordsPanel"},
            {"id": "PrivacyLLM", "preview": "assets/previews/07_privacy_llm.png", "unityPanel": "PrivacyPanel"},
        ],
        "stateMap": {
            "Normal": {"panel": "MainTownPanel", "catAnimation": "IdleBreath", "bubble": "先不用急，我在这里。"},
            "Transition": {"panel": "MainTownPanel", "catAnimation": "CuriousSniff", "bubble": "你慢下来了，我也安静一点。"},
            "Focus": {"panel": "FocusOverlay", "catAnimation": "IdleBreath", "bubble": "我会轻轻陪着你，不打扰。"},
            "Reward": {"panel": "RewardPanel", "catAnimation": "TailWagHappy", "bubble": "完成啦，猫咪给你一个小爪印。"},
            "DistractionNudge": {"panel": "MainTownPanel", "catAnimation": "HeadTiltListen", "bubble": "要不要回到刚才那件事？"},
        },
        "chatBubble": {
            "unityObject": "Canvas/MainTownPanel/CatChatBubble",
            "anchor": "CatRoot/BubbleAnchor",
            "mode": "state_reminder_or_encouragement",
            "autoHideSeconds": 4.5,
            "cooldownSeconds": 8.0,
            "screenOffset": [0, -110],
            "rules": [
                {"state": "Normal", "trigger": "enter_town_or_long_idle", "message": "先不用急，我在这里。"},
                {"state": "Transition", "trigger": "user_slows_down_before_focus", "message": "你慢下来了，我也安静一点。"},
                {"state": "Focus", "trigger": "focus_started_or_stable", "message": "我会轻轻陪着你，不打扰。"},
                {"state": "DistractionNudge", "trigger": "high_distraction_score", "message": "要不要回到刚才那件事？"},
                {"state": "Reward", "trigger": "focus_completed", "message": "完成啦，猫咪给你一个小爪印。"}
            ]
        },
        "cameraControls": {
            "mode": "fixed_height",
            "rotate": "360 degree in-place yaw around Y axis",
            "move": "forward/back along horizontal camera forward vector",
            "height": "locked to initial cameraRig.position.y",
            "buttons": [
                {"id": "RotateLeftButton", "label": "左旋", "handlerDown": "HoldRotateLeft", "handlerUp": "StopRotate"},
                {"id": "RotateRightButton", "label": "右旋", "handlerDown": "HoldRotateRight", "handlerUp": "StopRotate"},
                {"id": "MoveForwardButton", "label": "前进", "handlerDown": "HoldMoveForward", "handlerUp": "StopMove"},
                {"id": "MoveBackButton", "label": "后退", "handlerDown": "HoldMoveBack", "handlerUp": "StopMove"}
            ]
        },
    }
    (LAYOUT / "catlife_ui_layout.json").write_text(json.dumps(layout, ensure_ascii=False, indent=2), encoding="utf-8")

    rows = []
    for p in sorted(KIT.rglob("*")):
        if p.is_file() and p.name != "manifest.csv":
            h = hashlib.sha256(p.read_bytes()).hexdigest().upper()
            rows.append([str(p.relative_to(KIT)), p.stat().st_size, h])
    with (KIT / "manifest.csv").open("w", newline="", encoding="utf-8-sig") as f:
        writer = csv.writer(f)
        writer.writerow(["relative_path", "size_bytes", "sha256"])
        writer.writerows(rows)


if __name__ == "__main__":
    save_assets()
    print(f"Generated CatLife UI kit at {KIT}")
