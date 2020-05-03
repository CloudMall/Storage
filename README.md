# storage

## Intro

storage service for Cloud Mall

## Docker

``` bash
# remove before docker image if exists
# docker rmi cloudmall/storage:latest

docker run -d -p 5050:80 --name storage-service cloudmall/storage:latest

docker run -d -p 5050:80 -v /etc/configs/storage.appsettings.json:/app/appsettings/Production.json cloudmall/storage:latest
```

## Api

```
POST /api/storage/files
POST /api/storage/images
```

返回值：

错误示例：

``` json
{
  "result": null,
  "status": 400,
  "errorMsg": "文件上传失败"
}
```

正确示例：

``` json
{
  "result": "https://images.cloudmall.com/20200503/223322_0909.jpg",
  "status": 200,
  "errorMsg": null
}
```

status 为 200 代表操作成功，其他一般均为操作失败，status 状态码参考 <https://github.com/WeihanLi/WeihanLi.Common/blob/dev/src/WeihanLi.Common/Models/ResultModel.cs#L68>，具体错误信息参考 errorMsg 
