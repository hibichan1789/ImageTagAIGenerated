import json
import os
from openai import AsyncOpenAI
import base64
import logging


system_prompt = """"Your task:
- Generate image tags and assign a Tailwind CSS background color for each tag.
 
Rules:
- Output must be JSON only (no explanation, no text).
- Each tag must be a short Japanese noun.
- Generate 3 to 5 tag-color pairs.
- Each tag must have exactly one bgColor.
 
Color selection rules:
- bg-red-700: strong, danger, passion, intense objects (e.g., fire, car, action)
- bg-blue-700: calm, water, sky, cool atmosphere
- bg-green-700: nature, plants, outdoor environments
- bg-orange-800: warm, active, friendly things (e.g., animals, people)
- bg-violet-700: mysterious, creative, abstract, night scenes
 
Prefer consistent color mapping:
- Animals → bg-orange-800
- Nature → bg-green-700
- Water/sky → bg-blue-700
- Action objects → bg-red-700
- Night/abstract → bg-violet-700
 
Additional constraints:
- bgColor must be one of the following values ONLY:
  ["bg-red-700","bg-blue-700","bg-green-700","bg-orange-800","bg-violet-700"]
- Do not generate any value outside these options.
- Ensure color selection matches the meaning of each tag.
- Avoid assigning the same color to all tags; try to distribute colors appropriately.
 
Output format:
{
  "items": [
    { "tag": "犬", "bgColor": "bg-orange-800" },
    { "tag": "公園", "bgColor": "bg-green-700" },
    { "tag": "散歩", "bgColor": "bg-blue-700" }
  ]
}
"""


def encode_image(image_path:str):
    with open(image_path, "rb") as img:
        return base64.b64encode(img.read()).decode("utf-8")

client = AsyncOpenAI(api_key=os.environ["OPENAI_API_KEY"])
async def fetch_chat_response(ai_image_path:str):
    """
    OpenAI APIを非同期で呼び出す関数
    """
    base64_img = encode_image(ai_image_path)

    logging.info(f"Calling OpenAI with image_path = {ai_image_path}")

    response = await client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role": "system", "content": system_prompt},
            {
                "role": "user",
                "content": [
                    {
                        "type": "image_url",
                        "image_url": {"url": f"data:image/png;base64,{base64_img}"}
                    },
                    {"type": "text", "text": "この画像をタグ付けしてください。"}
                ]
            }
        ]
    )

    # 戻り値がnullの場合もある
    raw = response.choices[0].message.content

    logging.info(f"OpenAI raw response: {raw}")

    if raw is not None:
        clean_json = raw.replace("```json", "").replace("```", "").strip()
        return json.loads(clean_json)
    return None