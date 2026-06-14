import azure.functions as func
import logging
import os
from azure.storage.blob import BlobClient

from utils.image_function import generate_thumbnail, generate_ai_image
from models.file_process_model import FileProcessRequest, Status
from db.connection import get_connection
app = func.FunctionApp()

# blob保存用関数
def upload_to_blob(local_path: str, blob_name: str) -> str:
    blob = BlobClient.from_connection_string(
        conn_str=os.environ["FILE_STORAGE_CONNECTION"],
        container_name=os.environ["FILE_STORAGE_CONTAINER"],
        blob_name=blob_name
    )
    with open(local_path, "rb") as data:
        blob.upload_blob(data, overwrite=True)

    return blob.url

@app.queue_trigger(arg_name="azqueue", queue_name="file-process-queue", connection="0c6412_STORAGE") 
def file_process_queue(azqueue: func.QueueMessage):
    body = azqueue.get_body().decode("utf-8")
    logging.info(f"Queue body: {body}")
    
    file_process_request = FileProcessRequest.model_validate_json(body)
    file_id = file_process_request.file_id
    logging.info(f"Python Queue trigger received fileId={file_id}")

    # DB接続
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute(
            """
            SELECT "Id", "OriginalFileName","FileUrl", "UniqueFileName", "FileExtension", "Status"
            FROM "Files"
            WHERE "Id" = %s
            """,
            (file_id,)
        )
        row = cursor.fetchone()

        if row is None:
            logging.warning(f"レコードが見つかりません file_id={file_id}")
        else:
            (id, original_file_name, file_url, unique_file_name, file_extension, status) = row
            logging.info(f"id={id}, original_file_name={original_file_name}, file_url={file_url}, unique_file_name={unique_file_name}, file_extension={file_extension}, status={status}")
        # file_urlからストレージを検索してoriginal画像の取得をする
        # file_url = /images/originals/UniqueFileName.(画像の拡張子)
        # だから画像保存用のルートフォルダーをasp.net core apiとqueue functionsで共通化しないとだめ
        # あとからレコードに登録するThumbnailFileUrl,AiProcessedFileUrlの名前はuniqueFileNameに接頭辞を付ける方があとからapi側と合わせやすいかもしれない

        cursor.close()
        conn.close()
    except Exception as e:
        logging.error(f"Error: file_id = {file_id}, {e}")
        return

    # original画像のパスを組み立てる
    # Local or Azure
    mode = os.environ["FILE_STORAGE_MODE"]
    if mode == "Local":
        wwwroot = os.environ["WWWROOT_PATH"]
        original_file_path = os.path.join(wwwroot, file_url.lstrip("/"))
        logging.info(f"original_file_path = {original_file_path}")
    else:
        blob = BlobClient.from_connection_string(
            conn_str=os.environ["FILE_STORAGE_CONNECTION"],
            container_name=os.environ["FILE_STORAGE_CONTAINER"],
            blob_name=f"originals/{unique_file_name}"
        )
        temp_path = f"/tmp/{unique_file_name}"
        with open(temp_path, "wb") as f:
            f.write(blob.download_blob().readall())
        original_file_path = temp_path
        wwwroot = "/tmp"



    try:
        # wwwrootからの相対パス
        thumbnail_url = generate_thumbnail(original_file_path, wwwroot, unique_file_name, file_extension)
        ai_url = generate_ai_image(original_file_path, wwwroot, unique_file_name)

        # 実装のファイルの絶対パス
        ai_local_path = os.path.join(wwwroot, ai_url.lstrip('/'))
        thumbnail_local_path = os.path.join(wwwroot, thumbnail_url.lstrip('/'))

        logging.info(f"サムネイル画像生成: {thumbnail_local_path}")
        logging.info(f"OpenAI用画像生成: {ai_local_path}")

        if mode == "Local":
            thumbnail_db_url = thumbnail_url
            ai_db_url = ai_url
        else:
            # Azure: /tmpにあるファイルをBlobにアップロードしてそのURLをDBに保存する
            thumbnail_blob_name = f"thumbnails/{os.path.basename(thumbnail_local_path)}"
            ai_blob_name = f"ai/{os.path.basename(ai_local_path)}"

            thumbnail_db_url = upload_to_blob(thumbnail_local_path, thumbnail_blob_name)
            ai_db_url = upload_to_blob(ai_local_path, ai_blob_name)

            logging.info(f"サムネイル Blob URL: {thumbnail_db_url}")
            logging.info(f"AI画像 Blob URL: {ai_db_url}")
    except Exception as e:
        logging.error(f"画像生成失敗: file_id={file_id}, {e}")
        # 画像生成エラー時はStatusをerrorにする
        try:
            conn = get_connection()
            cursor = conn.cursor()
            cursor.execute(
                """
                UPDATE "Files"
                SET "Status" = %s
                WHERE "Id" = %s
                """,
                (Status.Error.value, file_id,)
            )
            conn.commit()
            cursor.close()
            conn.close()
        except:
            pass
        return

    # DBにURLとStatusを更新
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute(
            """
            UPDATE "Files"
            SET "ThumbnailFileUrl" = %s, "AiProcessedFileUrl" = %s, "Status" = %s
            WHERE "Id" = %s
            """,
            (thumbnail_db_url, ai_db_url, Status.ReadyForTag.value, file_id)
        )
        conn.commit()
        cursor.close()
        conn.close()

        logging.info(f"DB 更新成功: file_id={file_id}")
    except Exception as e:
        logging.error(f"DB更新失敗: file_id={file_id}, {e}")