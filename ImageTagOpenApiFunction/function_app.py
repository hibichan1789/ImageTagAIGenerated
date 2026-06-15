import azure.functions as func
import logging
import json
from typing import List
from models.dotnet_api_models import AiTagRequest, AiTagResponse, AiTagItem

app = func.FunctionApp(http_auth_level=func.AuthLevel.FUNCTION)

from services.file_service import get_ai_image_path
from services.open_ai_request import fetch_chat_response


@app.route(route="generate_tags")
async def generate_tags(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    
    body = req.get_json()
    logging.info(f"Request Body: {body}")
    ai_tag_request = AiTagRequest(**body)

    # ai処理用のurlの取得 例:ai_processed_file_url= /images/ai/ai_*.png
    ai_processed_file_url = ai_tag_request.ai_processed_file_url
    logging.info(f"ai_processed_file_url : {ai_processed_file_url}")
    # 環境変数www_rootを作って、環境変数からpathをファイルのルートからpathを作成して、画像の取得をする
    ai_image_path = get_ai_image_path(ai_processed_file_url)
    if ai_image_path is None:
        return func.HttpResponse(status_code=404)
    
    # openAI APIにシステムプロンプトと画像を投げる
    ai_response = await fetch_chat_response(ai_image_path)
    if ai_response is None:
        return func.HttpResponse(status_code=500)
    valid_response = AiTagResponse(**ai_response)

    clean_items:List[AiTagItem] = []
    for raw_item in valid_response.items:
        clean_items.append(
            AiTagItem(tag=raw_item.tag.strip(), bgColor=raw_item.bgColor.strip())
        )
    clean_response = AiTagResponse(items=clean_items)
    
    return func.HttpResponse(
            clean_response.model_dump_json(ensure_ascii=False),
            status_code=200,
            mimetype="application/json"
        )