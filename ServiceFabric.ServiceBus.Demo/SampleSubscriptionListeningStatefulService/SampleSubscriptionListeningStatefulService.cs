﻿using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using ServiceFabric.ServiceBus.Services;
using ServiceFabric.ServiceBus.Services.CommunicationListeners;

namespace SampleSubscriptionListeningStatefulService
{
	/// <summary>
	/// The FabricRuntime creates an instance of this class for each service type instance.
	/// </summary>
	internal sealed class SampleSubscriptionListeningStatefulService : StatefulService
	{
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			// In the configuration file, define connection strings: 
			// "Microsoft.ServiceBus.ConnectionString.Receive"
			// and "Microsoft.ServiceBus.ConnectionString.Send"

			// Also, define Topic & Subscription Names:
			string serviceBusTopicName = CloudConfigurationManager.GetSetting("TopicName");
			string serviceBusSubscriptionName = CloudConfigurationManager.GetSetting("SubscriptionName");

			yield return new ServiceReplicaListener(parameters => new ServiceBusSubscriptionCommunicationListener(
				new Handler(this)
				, parameters
				, serviceBusTopicName
				, serviceBusSubscriptionName));
		}

		internal sealed class Handler : AutoCompleteServiceBusMessageReceiver
		{
			private readonly StatefulService _service;

			public Handler(StatefulService service)
			{
				_service = service;
			}

			protected override void ReceiveMessageImpl(BrokeredMessage message, CancellationToken cancellationToken)
			{
				ServiceEventSource.Current.ServiceMessage(_service, $"Handling queue message {message.MessageId}");
			}
		}
	}
}
