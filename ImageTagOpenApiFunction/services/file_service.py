import os
from azure.storage.blob import BlobClient
import logging


def get_ai_image_path(ai_processed_file_url:str)->str|None:
    mode = os.environ["FILE_STORAGE_MODE"]
    logging.info(f"FILE_STORAGE_MODE = {mode}")
    if mode == "Local":
        wwwroot = os.environ["WWWROOT_PATH"]
        ai_image_path = os.path.join(wwwroot, ai_processed_file_url.lstrip("/"))
        # ai_image_pathは絶対パスだからファイルの存在確認できる
        if os.path.exists(ai_image_path):
            return ai_image_path
        # Noneの場合はStatusCodeをエラー番号で返せばいい
        return None
    else:
        # Azure Blobからダウンロード
        logging.info(f"ai_processed_file_url = {ai_processed_file_url}")
        blob_name = f"ai/{os.path.basename(ai_processed_file_url)}"
        logging.info(f"Downloading blob: container={os.environ['FILE_STORAGE_CONTAINER']}, blob={blob_name}")
        blob = BlobClient.from_connection_string(
            conn_str=os.environ["FILE_STORAGE_CONNECTION"],
            container_name=os.environ["FILE_STORAGE_CONTAINER"],
            blob_name=blob_name # ai/ai_*.png
        )
        
        


        # /tmp/ai_*.png にblobのファイルを書き込んで書き込んだパスを返す
        tmp_path = f"/tmp/{os.path.basename(ai_processed_file_url)}"
        logging.info(f"Saving to tmp_path = {tmp_path}")
        with open(tmp_path, "wb") as f:
            f.write(blob.download_blob().readall())
        return tmp_path