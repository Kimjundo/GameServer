using System.Net;
using System.Net.Sockets;

public class FirstClass
{
    public void MyMethod()
    {
        int a = 5 ; // <- 여기서 세미콜론이 빠진 경우
        Console.WriteLine(a);
    }
}


public class CNetworkService
{
    //클라이언트의 접속을 받아들이기 위한 객체 
    CListener client_listener;

    //메세지 수신, 전송 시 필요한 객체
    SocketAsyncEventArgsPool receive_event_args_pool;
    SocketAsyncEventArgsPool send_event_args_pool;
    
    //메시지 수신, 전송 시 닷넷 비동기 소켓에서 사용할 버퍼를 관리하는 객체
    BufferManager buffer_manager;

    //클라이언트의 접속이 이루어졌을때 호출되는 델리게이트 
    public delegate void SessionHandler(CUserToken token);
    public SessionHandler session_created_callback { get; set; }


}

class CListener
{
    //비동기 Accept를 위한 EventArgs
    SocketAsyncEventArgs accept_args;

    //클라이언트의 접속을 처리할 소켓
    Soket listen_soket;

    //Accept 처리의 순서를 제어하기 위한 이벤트 변수
    AutoResetEvent flow_control_event;

    //새로운 클라이언트가 접속했을 때 호출되는 델리게이트 
    public delegate void NewclientHandler(Socket client_socket, object token);

    public NewclientHandler callback_on_newclient;

public CListener()
    {
        this.callback_on_newclient = null;
    }

    public void start(string host, int port,int backlog)
    {
        //소켓을 생성한다.
        this.listen_soket = new Soket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);


        IPAddress address;
        if(host == "0.0.0.0")
        {
            address = IPAddress.Any; 
        }
        else {
            address = IPAddress.Parse(host);
        }
        IPEndPoint endpoint = new IPEndPoint(address, port);

        try
        {
            //소켓에 host 정보를 바인딩시킨 뒤 Listen 메소드를 호출하여 대기한다.
            this.listen_soket.Bind(endpoint);
            this.listen_soket.Listen(backlog);


            this.accept_args = new SocketAsyncEventArgs();
            this.accept_args.Completed += new EventHandler<SocketAsyncEventArgs>(on_accept_completed);

            //클라이언트가 들어오기를 기다린다.
            //비동기 메서드이므로 블로킹되지 않고 바로 리턴되며
            //콜백 메서드를 통해서 접속 통보를 받는다.
            this.listen_soket.AcceptAsync(this.accept_args);
        }
}