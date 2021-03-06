using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using Obvs.Types;

namespace Obvs.Tests
{
    [TestFixture]
    public class TestTypeRoutingMessagePublisher
    {
        [Test]
        public void ShouldDispatchToCorrectPublisher()
        {
            IMessagePublisher<IMessage> eventPublisher = A.Fake<IMessagePublisher<IMessage>>();
            IMessagePublisher<IMessage> commandPublisher = A.Fake<IMessagePublisher<IMessage>>();
            IMessagePublisher<IMessage> messagePublisher = A.Fake<IMessagePublisher<IMessage>>();

            TypeRoutingMessagePublisher<IMessage> typeRoutingMessagePublisher =
                new TypeRoutingMessagePublisher<IMessage>(new[]
                {
                    new KeyValuePair<Type, IMessagePublisher<IMessage>>(typeof(IEvent), eventPublisher),
                    new KeyValuePair<Type, IMessagePublisher<IMessage>>(typeof(ICommand), commandPublisher),
                    new KeyValuePair<Type, IMessagePublisher<IMessage>>(typeof(IMessage), messagePublisher)
                });

            IEvent ev = A.Fake<IEvent>();
            ICommand command = A.Fake<ICommand>();
            IMessage message = A.Fake<IMessage>();

            typeRoutingMessagePublisher.PublishAsync(ev);
            typeRoutingMessagePublisher.PublishAsync(command);
            typeRoutingMessagePublisher.PublishAsync(message);

            A.CallTo(() => eventPublisher.PublishAsync(ev)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => eventPublisher.PublishAsync(message)).MustNotHaveHappened();
            A.CallTo(() => eventPublisher.PublishAsync(command)).MustNotHaveHappened();

            A.CallTo(() => commandPublisher.PublishAsync(ev)).MustNotHaveHappened();
            A.CallTo(() => commandPublisher.PublishAsync(message)).MustNotHaveHappened();
            A.CallTo(() => commandPublisher.PublishAsync(command)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => messagePublisher.PublishAsync(ev)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => messagePublisher.PublishAsync(command)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => messagePublisher.PublishAsync(message)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}