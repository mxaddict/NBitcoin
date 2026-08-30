using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NBitcoin.Altcoins
{
    public class Navio : NetworkSetBase
    {
        public static Navio Instance { get; } = new Navio();
        public override string CryptoCode => "NAV";

        private Navio()
        {
        }

        protected override void PostInit()
        {
            // Testnet's datadir is named for the chain generation, not "testnet":
            // navio-core src/chainparamsbase.cpp, CreateBaseChainParams.
            RegisterDefaultCookiePath("Navio",
                new FolderName() { TestnetFolder = "testnet7" });
        }

        /// <summary>
        /// Configures an RPCClient with BLSCT method name overrides for Navio.
        /// Call this after creating an RPCClient connected to a Navio daemon.
        /// </summary>
        public static void ConfigureBLSCTOverrides(RPCClient client)
        {
            client.RPCMethodOverrides = new Dictionary<string, string>
            {
                { "getbalance",                   "getblsctbalance" },
                { "sendtoaddress",                "sendtoblsctaddress" },
                { "listunspent",                  "listblsctunspent" },
                { "listtransactions",             "listblscttransactions" },
                { "createrawtransaction",         "createblsctrawtransaction" },
                { "fundrawtransaction",           "fundblsctrawtransaction" },
                { "signrawtransactionwithwallet", "signblsctrawtransaction" },
                { "decoderawtransaction",         "decodeblsctrawtransaction" },
            };
        }

        protected override NetworkBuilder CreateTestnet()
        {
            return new NetworkBuilder()
                .SetConsensus(new Consensus()
                {
                    SubsidyHalvingInterval = 210000,
                    MajorityEnforceBlockUpgrade = 51,
                    MajorityRejectBlockOutdated = 75,
                    MajorityWindow = 100,
                    PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60),
                    PowTargetSpacing = TimeSpan.FromSeconds(10 * 60),
                    PowAllowMinDifficultyBlocks = true,
                    PowNoRetargeting = true,
                    CoinbaseMaturity = 100,
                    ConsensusFactory = new ConsensusFactory(),
                    SupportSegwit = true,
                    SupportTaproot = true,
                })
                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 111 })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 196 })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 239 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tb"))
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tb"))
                .SetMagic(0xc1d26724)
                .SetPort(33670)
                .SetRPCPort(33677)
                .SetMaxP2PVersion(70016)
                .SetName("nav-test")
                .SetNetworkStringParser(new NetworkStringParser())
                // Real 80-byte header from navio-core CTestNetParams — including the
                // merkle root and the nonce the genesis grind settled on — so
                // Consensus.HashGenesisBlock is the chain's actual genesis hash, which
                // is all NBitcoin derives from these bytes. The body is a placeholder
                // coinbase: the real one is a BLSCT transaction, and Navio's outpoints
                // carry no output index (src/primitives/transaction.h, COutPoint), so
                // no Navio transaction round-trips through NBitcoin's parser. Read
                // GetGenesis().Transactions as fiction; the hash is the contract.
                .SetGenesis("0000004000000000000000000000000000000000000000000000000000000000000000007f500ca40cc52d94a1f106d06f735735266fa603d16f36f77ed8ed52ea4b0586d237f269ffff7f20010000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff00ffffffff010000000000000000016a00000000");
        }

        protected override NetworkBuilder CreateMainnet()
        {
            return new NetworkBuilder()
                .SetConsensus(new Consensus()
                {
                    SubsidyHalvingInterval = 210000,
                    MajorityEnforceBlockUpgrade = 750,
                    MajorityRejectBlockOutdated = 950,
                    MajorityWindow = 1000,
                    PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60),
                    PowTargetSpacing = TimeSpan.FromSeconds(10 * 60),
                    PowAllowMinDifficultyBlocks = false,
                    PowNoRetargeting = false,
                    CoinbaseMaturity = 100,
                    ConsensusFactory = new ConsensusFactory(),
                    SupportSegwit = true,
                    SupportTaproot = true,
                })
                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 0 })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 5 })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 128 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("bc"))
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("bc"))
                .SetMagic(0x00c35fbd)
                .SetPort(48470)
                .SetRPCPort(48471)
                .SetMaxP2PVersion(70016)
                .SetName("nav-main")
                .SetNetworkStringParser(new NetworkStringParser())
                // Real 80-byte header from navio-core CMainParams; body is a
                // placeholder for the same reason as testnet above.
                .SetGenesis("000000400000000000000000000000000000000000000000000000000000000000000000af9a2fee834348d5c92cce7ff809669343e55fe8424b9dbc1230433cccdff896500f456affff7f20000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff00ffffffff010000000000000000016a00000000");
        }

        protected override NetworkBuilder CreateRegtest()
        {
            // Regtest has fBLSCT=false and is excluded from the Navio integration.
            return null;
        }
    }
}