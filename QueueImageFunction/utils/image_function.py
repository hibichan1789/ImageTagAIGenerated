import os
from PIL import Image, ImageOps

def normalize_extension(file_extension:str)->str:
    ext = file_extension
    if not ext.startswith("."):
        ext = f".{ext}"
    return ext

def generate_thumbnail(original_file_path:str, www_root:str, unique_file_name:str, file_extension:str)->str:
    """
    サムネイル画像を生成して保存して、DB保存用のThumbnailFileUrlを返す
    例: /images/thumbnails/thumb_xxxx.png
    """
    # 拡張子を小文字に正規化
    ext = normalize_extension(file_extension)
    
    # 保存ファイル名とパス
    thumb_relative_dir = "/images/thumbnails"
    thumb_file_name =  f"thumb_{unique_file_name}" if "." in unique_file_name else f"thumb_{unique_file_name}{ext}"

    thumb_file_url = f"{thumb_relative_dir}/{thumb_file_name}"
    thumb_save_dir = os.path.join(www_root, "images", "thumbnails")
    os.makedirs(thumb_save_dir, exist_ok=True)
    save_path = os.path.join(thumb_save_dir, thumb_file_name)

    # 画像を開いてサムネイルの作成
    # 中央から横長長方形にフロントエンドで使えるように画像を整える
    width = 960
    height = int((width*9)/16)
    size = (width, height)
    with Image.open(original_file_path) as img:
        thumbnail_img = ImageOps.fit(img, size, method=Image.Resampling.LANCZOS)
        thumbnail_img.save(save_path, quality=80)
    
    return thumb_file_url

def generate_ai_image(original_file_path:str, www_root:str, unique_file_name:str)->str:
    """
    AI用の画像を生成して保存してDB用のAiProcessedFileUrlを返す
    OpenAIが扱いやすい形式に成形(正方形,512px)
    """
    # 保存ファイル名とパス
    unique_file_name_noext = unique_file_name.split(".")[0]
    ai_relative_dir = "/images/ai"
    ai_file_name =  f"ai_{unique_file_name_noext}.png"
    ai_file_url = f"{ai_relative_dir}/{ai_file_name}"

    ai_save_dir = os.path.join(www_root, "images", "ai")
    os.makedirs(ai_save_dir, exist_ok=True)
    save_path = os.path.join(ai_save_dir, ai_file_name)

    # AI用画像生成
    with Image.open(original_file_path) as img:
        width, height = img.size

        # 正方形に中央切り取り
        min_edge = min(width, height)
        left = (width - min_edge)//2
        top = (height - min_edge)//2
        right = left + min_edge
        bottom = top + min_edge
        img_cropped = img.crop((left, top, right, bottom))

        # リサイズしてPNGで保存
        img_resized = img_cropped.resize((512, 512), Image.Resampling.LANCZOS)
        img_resized.save(save_path, format="PNG", optimize=True)
    return ai_file_url