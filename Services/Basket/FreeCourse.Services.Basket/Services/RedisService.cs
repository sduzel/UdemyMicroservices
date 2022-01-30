using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Services
{
    public class RedisService
    {
        private readonly string _host;
        private readonly int _port;

        ConnectionMultiplexer _connectionMultiplexer;//redis ile iletişim kurmasını sağlayan sınıf. ExchangeRedis paketinden geliyor.
        public RedisService(string host,int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect() => _connectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");
        public IDatabase GetDb(int db = 1) => _connectionMultiplexer.GetDatabase(db); //Redis te birden fazla db var 1-10 test için hangisini kullanmak istersen

    }
}
