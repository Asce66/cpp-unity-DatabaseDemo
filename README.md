

## **基于cpp+unity的前后端数据库Demo**

一个数据库的Demo，c++做为服务端，unity做为客户端，使用mysql数据库保存数据，protobuf进行协议存储与解析

#### c++服务器:

- 连接Mysql数据库，进行用户数据的增删改查
- 使用protobuf进行通信协议解析 协议格式为：协议长度+协议ID+协议内容
- 对原生socket进行一下小的封装，更加便利与安全
- 使用select实现非阻塞I/O
- 使用map+智能指针+protobuf反射的方式实现不同协议的分发和处理
- 封装一个可变长度的char数组接收消息，解决消息的粘包和分包问题

#### unity客户端：

- 封装一个可变长度的byte数组接收消息，解决粘包分包问题
- 使用Dictionary+C#反射实现不同协议的分发和处理
- 使用事件监听处理登录、连接、断开连接等事件

#### 部分效果展示



![image-20201110125755029](https://i.loli.net/2020/11/10/fBDwYxNVtOlPQRT.png)



<img src="https://i.loli.net/2020/11/10/nAF49DPGwHIR83a.png" alt="image-20201110125933843" style="zoom:67%;" />



<img src="C:\Users\Asce\AppData\Roaming\Typora\typora-user-images\image-20201110130050417.png" alt="image-20201110130050417" style="zoom: 50%;" />



<img src="C:\Users\Asce\AppData\Roaming\Typora\typora-user-images\image-20201110130123227.png" alt="image-20201110130123227" style="zoom:67%;" />

<img src="C:\Users\Asce\AppData\Roaming\Typora\typora-user-images\image-20201110130159893.png" alt="image-20201110130159893" style="zoom: 50%;" />



#### 云服务器的服务端配置

0. 在云服务器上配置客户端和服务端通信的对应端口(我代码里写的是**8866**,当然也可以自己修改)**出/入方向**，都是**TCP**协议

1. 下载并安装**Mysql数据库**[下载地址](https://dev.mysql.com/downloads/installer/)（PS:选择体积大的那个下载，小的是用于web的）
2. 如果.exe的同级目录下没有这两个dll就在**C:\Program Files\MySQL\Connector C++ 8.0\lib64**下找到并复制过来

![image-20201110124131832](https://i.loli.net/2020/11/10/meyNG4Aqv2ahD6t.png)

3. 找到文件夹下的backup.sql文本，这是一个**备份**的Mysql数据库文件，使用它来恢复基本的数据库信息，用于服务端的数据库连接。Mysql如何进行数据备份和恢复可以看[这个](https://github.com/Asce66/MyNotes/blob/master/%E6%9D%82/Mysql.md)

4. 启动服务器的.exe文件，如果提示缺少什么dll就直接从本机的**C:\Windows\System32**下找到对应的文件拷贝过去

5. 在命令行中输入对应云服务器的IP地址,一定是云服务器的**私有IP**，127.0.0.1和公有ip都不行（PS:客户端连接云服务器使用的就是公有IP了），出现下图的**服务器已启动**就表示服务器开始正常工作了

   ![image-20201110124729392](https://i.loli.net/2020/11/10/qBiySZGtVjbXsl7.png)

#### 客户端设置

在物体组件上设置服务端的IP地址和端口号

<img src="C:\Users\Asce\AppData\Roaming\Typora\typora-user-images\image-20201110130515789.png" alt="image-20201110130515789" style="zoom: 67%;" />