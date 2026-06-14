from pydantic import BaseModel
from typing import List

class AiTagRequest(BaseModel):
    ai_processed_file_url:str

class AiTagItem(BaseModel):
    tag:str
    bgColor:str

class AiTagResponse(BaseModel):
    items:List[AiTagItem]