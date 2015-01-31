﻿using System.Collections.Generic;
using WampSharp.Core.Contracts;
using WampSharp.Core.Message;

namespace WampSharp.V2.Core.Contracts
{
    /// <summary>
    /// Represents a proxy to a WAMP2 router session handler.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <remarks>These messages are part of the WAMP2 specification.</remarks>
    public interface IWampSessionProxy<TMessage>
    {
        /// <summary>
        /// Sends a HELLO message.
        /// </summary>
        /// <param name="realm">The requested realm to join.</param>
        /// <param name="details">Details about the client.</param>
        [WampHandler(WampMessageType.v2Hello)]
        void Hello(string realm, TMessage details);

        /// <summary>
        /// Sends a ABORT message.
        /// </summary>
        /// <param name="details">Additional details.</param>
        /// <param name="reason">A uri representing the abort reason.</param>
        [WampHandler(WampMessageType.v2Abort)]
        void Abort(AbortDetails details, string reason);

        /// <summary>
        /// Sends an AUTHENTICATE message.
        /// </summary>
        /// <param name="signature">A signature.</param>
        /// <param name="extra">Extra data.</param>
        [WampHandler(WampMessageType.v2Authenticate)]
        void Authenticate(string signature, IDictionary<string, object> extra);

        /// <summary>
        /// Sends a GOODBYE message.
        /// </summary>
        /// <param name="details">Additional details.</param>
        /// <param name="reason">A uri representing the leave reason.</param>
        [WampHandler(WampMessageType.v2Goodbye)]
        void Goodbye(TMessage details, string reason);

        /// <summary>
        /// Sends a HEARTBEAT message.
        /// </summary>
        /// <param name="incomingSeq">The incoming count sequence.</param>
        /// <param name="outgoingSeq">The outgoing count sequence.</param>
        [WampHandler(WampMessageType.v2Heartbeat)]
        void Heartbeat(int incomingSeq, int outgoingSeq);

        /// <summary>
        /// Sends a HEARTBEAT message.
        /// </summary>
        /// <param name="incomingSeq">The incoming count sequence.</param>
        /// <param name="outgoingSeq">The outgoing count sequence.</param>
        /// <param name="discard">???</param>
        [WampHandler(WampMessageType.v2Heartbeat)]
        void Heartbeat(int incomingSeq, int outgoingSeq, string discard);         
    }
}