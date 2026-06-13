import azure.functions as func
import logging
import os

from utils.image_function import generate_thumbnail, generate_ai_image
from models.file_process_model import FileProcessRequest, Status
from db.connection import get_connection
app = func.FunctionApp()



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
    wwwroot = os.environ["WWWROOT_PATH"]
    original_file_path = os.path.join(wwwroot, file_url.lstrip("/"))
    logging.info(f"original_file_path = {original_file_path}")

    try:
        thumbnail_url = generate_thumbnail(original_file_path, wwwroot, unique_file_name, file_extension)
        ai_url = generate_ai_image(original_file_path, wwwroot, unique_file_name)

        logging.info(f"サムネイル画像生成: {os.path.join(wwwroot, thumbnail_url.lstrip('/'))}")
        logging.info(f"OpenAI用画像生成: {os.path.join(wwwroot, ai_url.lstrip('/'))}")
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
            (thumbnail_url, ai_url, Status.ReadyForTag.value, file_id)
        )
        conn.commit()
        cursor.close()
        conn.close()

        logging.info(f"DB 更新成功: file_id={file_id}")
    except Exception as e:
        logging.error(f"DB更新失敗: file_id={file_id}, {e}")