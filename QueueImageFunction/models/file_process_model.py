from enum import Enum
from pydantic import BaseModel

class FileProcessRequest(BaseModel):
    file_id:int

class Status(Enum):
    Processing = 0
    ReadyForTag = 1
    Completed = 2
    Error = 3