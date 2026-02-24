# 엔트리 포인트
-
* Program 클래스 내부에 Main 함수가 실행되는 방식
```shell
dotnet run --project ServerCore\ServerCore.csproj
dotnet watch --project ServerCore\ServerCore.csproj
```
서버 실행

# 소켓 vs 패킷 vs 세션

### 서버 -> 클라이언트, 클라이언트 -> 서버
* 서버, 클라이언트 : 엔드포인트 (목적지)
* 데이터 (protobuf binary 데이터 단위) : 패킷 (택배)
* 통신 인터페이스 (연결 회로) : 소켓 (전화)
* 통화 대상 및 그 연결 자체 : 세션 (논리적 연결)
