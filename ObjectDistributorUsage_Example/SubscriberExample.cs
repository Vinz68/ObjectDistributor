using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeSoft.ObjectDistributor;

namespace ObjectDistributorUsage_Example
{
    class SubscriberExample : IDisposable
    {
        public SubscriberExample(IObjectDistributor distributor)
        {
            _distributor = distributor;

            // Subscribe to MessageExample1 and MessageExample2
            _distributor.Subscribe(typeof(MessageExample1), MessageExample1Handler);
            _distributor.Subscribe(typeof(MessageExample2), MessageExample2Handler); 
        }



        private void MessageExample1Handler(DistributorNotification distributorNotification)
        {
            var msgObject = (distributorNotification.MessageObject as MessageExample1)!;

            Console.WriteLine($"MessageExample1Handler received a notification: Message = {msgObject.Message}");
        }

        private void MessageExample2Handler(DistributorNotification distributorNotification)
        {
            var msgObject = (distributorNotification.MessageObject as MessageExample2)!;


            Console.WriteLine($"MessageExample2Handler received a notification. Result =  {msgObject.Result}");
        }

        public void Dispose()
        {
            _distributor.UnSubscribeAll(this);
        }

        private IObjectDistributor _distributor;
    }
}
