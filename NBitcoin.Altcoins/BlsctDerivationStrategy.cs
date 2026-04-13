using System;
using NBitcoin.DataEncoders;
using NavioBlsct;

namespace NBitcoin.Altcoins
{
    /// <summary>
    /// Client-side BLSCT sub-address derivation via navio-core external C API.
    /// Calls: gen_sub_addr_id → derive_sub_address → encode_address.
    /// </summary>
    public static class BlsctAddressDeriver
    {
        /// <param name="viewKeyBytes">32 bytes (from getblsctauditkey, first 64 hex chars)</param>
        /// <param name="spendKeyBytes">48 bytes (from getblsctauditkey, next 96 hex chars)</param>
        /// <param name="account">0 = receive, -1 = change, -2 = staking</param>
        /// <param name="index">Increments per address</param>
        /// <param name="encoding">Address encoding (Bech32 or Bech32M)</param>
        /// <returns>Encoded BLSCT address string</returns>
        public static string Derive(
            byte[] viewKeyBytes,
            byte[] spendKeyBytes,
            long account,
            ulong index,
            AddressEncoding encoding = AddressEncoding.Bech32M)
        {
            IntPtr id = IntPtr.Zero;
            IntPtr addr = IntPtr.Zero;
            try
            {
                id = Blsct.GenSubAddrId(account, index);
                addr = Blsct.DeriveSubAddress(viewKeyBytes, spendKeyBytes, id);
                return Blsct.EncodeAddress(addr, encoding);
            }
            finally
            {
                if (addr != IntPtr.Zero) Blsct.FreeObj(addr);
                if (id != IntPtr.Zero) Blsct.FreeObj(id);
            }
        }
    }

    /// <summary>
    /// NBitcoin DerivationStrategyBase for BLSCT wallets.
    /// String format: "blsct:VIEW_KEY_HEX:SPEND_KEY_HEX"
    ///   VIEW_KEY_HEX  = 64 hex chars (32 bytes, Fr scalar)
    ///   SPEND_KEY_HEX = 96 hex chars (48 bytes, G1 point)
    /// Obtained by: navio-cli getblsctauditkey
    /// </summary>
    public class BlsctDerivationStrategy : DerivationStrategyBase
    {
        public const string Prefix = "blsct:";
        public const long ChangeAccount = -1;
        public const long StakingAccount = -2;

        public byte[] ViewKey { get; } // 32 bytes
        public byte[] SpendKey { get; } // 48 bytes

        public BlsctDerivationStrategy(byte[] viewKey, byte[] spendKey)
            : base(null)
        {
            if (viewKey.Length != 32) throw new ArgumentException("View key must be 32 bytes");
            if (spendKey.Length != 48) throw new ArgumentException("Spend key must be 48 bytes");
            ViewKey = viewKey;
            SpendKey = spendKey;
        }

        public static BlsctDerivationStrategy Parse(string s)
        {
            if (!s.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)) return null;
            var parts = s.Substring(Prefix.Length).Split(':');
            if (parts.Length != 2 || parts[0].Length != 64 || parts[1].Length != 96) return null;
            try
            {
                return new BlsctDerivationStrategy(
                    Encoders.Hex.DecodeData(parts[0]),
                    Encoders.Hex.DecodeData(parts[1]));
            }
            catch
            {
                return null;
            }
        }

        public override string ToString() =>
            Prefix + Encoders.Hex.EncodeData(ViewKey) + ":" + Encoders.Hex.EncodeData(SpendKey);

        public override DerivationStrategyBase GetChild(int i) => this;

        public override Derivation Derive(uint index) =>
            throw new NotSupportedException(
                "Use BlsctAddressDeriver.Derive() for BLSCT address generation");
    }

    /// <summary>
    /// Factory for parsing BLSCT derivation strategy strings.
    /// </summary>
    public class BlsctDerivationStrategyFactory : DerivationStrategyFactory
    {
        public override DerivationStrategyBase Parse(string strategy)
        {
            var blsctStrategy = BlsctDerivationStrategy.Parse(strategy);
            if (blsctStrategy != null)
                return blsctStrategy;

            // Fall back to base implementation for other strategies
            return base.Parse(strategy);
        }
    }
}
