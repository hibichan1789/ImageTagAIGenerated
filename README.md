# 画像アップロード、AIタグ生成アプリ  

画像をアップロードし、タグ生成ボタンを押すとAIが画像を分析してタグの自動生成をしてくれるアプリ  


## 起動方法  
- cp .env.sample .env  
プロジェクトルートディレクトリでdocker-compose.yml用の.envを作成しの設定　　
- docker compose up -d  
Docker PostgreSQLの起動  
- docker ps  
- docker logs file-tag-postgres  
Docker PostgreSQLが起動しているか確認   


## 学んだこと  
C#のDateTimeOffsetは日時データに加えて、UTCとの時差（Offset）を正確に保持する  　
世界中のどこであっても「絶対的な一時点」を正確に識別できるため、ログの記録、DBへの保存、サーバー間での通信で最も安全　　