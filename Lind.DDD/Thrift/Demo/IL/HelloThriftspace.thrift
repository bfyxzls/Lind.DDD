namespace csharp HelloThriftspace
service HelloThrift{
  void HelloWorld(),
string GetData(1:i32 uid),
int adding(1:i32 a,2:i32 b)
}