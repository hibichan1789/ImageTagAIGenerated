import os
wwwroot = os.environ["WWWROOT_PATH"]
def get_ai_image_path(ai_processed_file_url:str)->str|None:
    ai_image_path = os.path.join(wwwroot, ai_processed_file_url.lstrip("/"))

    # ai_image_pathは絶対パスだからファイルの存在確認できる
    if os.path.exists(ai_image_path):
        return ai_image_path
    
    # Noneの場合はStatusCodeをエラー番号で返せばいい
    return None