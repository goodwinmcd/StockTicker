using System.Collections.Generic;

namespace RedditMonitorWorker.Models
{
    public class ListOfCommonTickers
    {
        public static IEnumerable<string> CommonTickerNames { get =>
            new HashSet<string>
            {
                "HALL",
                "PAC",
                "HALO",
                "CDC",
                "MMM",
                "NAT",
                "LL",
                "RAIL",
                "REG",
                "RAD",
                "DEN",
                "RAMP",
                "MJ",
                "DECK",
                "RICE",
                "NINE",
                "RICK",
                "HAIL",
                "PACK",
                "HACK",
                "ACES",
                "NAIL",
                "NGL",
                "GL",
                "ECON",
                "CENT",
                "RING",
                "NERD",
                "TREE",
                "TRIP",
                "GENE",
                "GEN",
                "COKE",
                "CO",
                "SNAP",
                "DOW",
                "DON",
                "DM",
                "STAR",
                "ARC",
                "RARE",
                "RACE",
                "COLD",
                "CORE",
                "CORN",
                "NP",
                "CORP",
                "DEF",
                "EFT",
                "O",
                "ONTO",
                "TOPS",
                "MOD",
                "MET",
                "ET",
                "INFO",
                "FOLD",
                "JAN",
                "LIVE",
                "SPY",
                "WISH",
                "WIL",
                "WIT",
                "SHY",
                "WIFI",
                "WIRE",
                "WING",
                "HR",
                "NICE",
                "ICE",
                "ON",
                "A",
                "ABNB",
                "NBA",
                "FOR",
                "SO",
                "SKY",
                "TWO",
                "TWIN",
                "NOAH",
                "ALL",
                "ALLY",
                "AHH",
                "ARE",
                "IT",
                "NOW",
                "NOV",
                "JACK",
                "CS",
                "CC",
                "MA",
                "MAC",
                "AC",
                "ANY",
                "NYT",
                "NYC",
                "BE",
                "BEN",
                "CAN",
                "CANE",
                "CAKE",
                "CAT",
                "CRY",
                "MARK",
                "SEE",
                "SEED",
                "OUT",
                "JUST",
                "STIM",
                "TILT",
                "ONE",
                "NEO",
                "MAN",
                "OR",
                "BIT",
                "BIO",
                "IQ",
                "QUAD",
                "IRS",
                "LOVE",
                "TELL",
                "GOOD",
                "DUST",
                "DUO",
                "DUAL",
                "UFO",
                "DUG",
                "TBF",
                "DASH",
                "HP",
                "DAN",
                "DARE",
                "AT",
                "IVE",
                "EBAY",
                "U",
                "UI",
                "GO",
                "EARN",
                "EARS",
                "HOLD",
                "HOOK",
                "KO",
                "PUMP",
                "YOLO",
                "Y",
                "BIG",
                "IG",
                "R",
                "AM",
                "AMP",
                "BRO",
                "ROBO",
                "ROOF",
                "ROOT",
                "RODE",
                "ROKU",
                "HAS",
                "SWAN",
                "WAT",
                "ALEX",
                "FIX",
                "C",
                "HE",
                "HERO",
                "HERD",
                "BEST",
                "TX",
                "TY",
                "FAN",
                "FAD",
                "FANG",
                "MLM",
                "FAB",
                "FARM",
                "UNIT",
                "FAM",
                "FACE",
                "UBER",
                "X",
                "K",
                "MOON",
                "POST",
                "POR",
                "POOL",
                "GAIN",
                "GAL",
                "AL",
                "AIM",
                "BY",
                "SUB",
                "SURF",
                "SUN",
                "BLUE",
                "LUV",
                "LULU",
                "WELL",
                "WRAP",
                "PHD",
                "PH",
                "FUN",
                "FURY",
                "FUSE",
                "MAS",
                "MAGA",
                "CAMP",
                "PLAY",
                "AY",
                "LOW",
                "VERY",
                "VET",
                "VETS",
                "BILL",
                "BIL",
                "HAS",
                "AN",
                "CEO",
                "EOD",
                "OPEN",
                "PEP",
                "ENG",
                "PETS",
                "PENN",
                "PEN",
                "PEG",
                "EGO",
                "IPO",
                "IPOD",
                "DDS",
                "SMH",
                "SMOG",
                "Z",
                "J",
                "FILL",
                "A",
                "MAIN",
                "WANT",
                "WASH",
                "THO",
                "THY",
                "HOG",
                "THC",
                "LIFE",
                "LITE",
                "NEW",
                "EW",
                "NEXT",
                "EXP",
                "WORK",
                "R",
                "REAL",
                "EA",
                "EAST",
                "TERM",
                "CASH",
                "CVS",
                "CALM",
                "ID",
                "EVER",
                "DD",
                "AGO",
                "GOAT",
                "GDP",
                "GOVT",
                "FREE",
                "FROG",
                "RUN",
                "UAVS",
                "UPS",
                "HOPE",
                "SHE",
                "HES",
                "ES",
                "TRUE",
                "TRU",
                "ELSE",
                "EL",
                "JOB",
                "JP",
                "OLD",
                "AWAY",
                "HUGE",
                "GURU",
                "COST",
                "COW",
                "TECH",
                "CHEF",
                "CYBER",
                "ECHO",
                "CHAD",
                "FUND",
                "MUST",
                "PLAN",
                "LAKE",
                "CARE",
                "MIND",
                "MINT",
                "INT",
                "MIC",
                "ICON",
                "MID",
                "CAR",
                "AQUA",
                "HOME",
                "STAY",
                "STAG",
                "YANG",
                "TACO",
                "TAIL",
                "PICK",
                "PIN",
                "PINE",
                "PING",
                "PINS",
                "NSA",
                "GOLD",
                "GOLF",
                "IMO",
                "K",
                "SAFE",
                "SAM",
                "SAN",
                "SAND",
                "SF",
                "SALT",
                "ALTS",
                "ALT",
                "PIE",
                "KIDS",
                "KIM",
                "GROW",
                "GRAY",
                "WY",
                "GRID",
                "GUT",
                "NET",
                "TH",
                "MAX",
                "EV",
                "VICE",
                "HEAR",
                "EAR",
                "DEEP",
                "TURN",
                "NEAR",
                "PLUS",
                "LUNG",
                "FAST",
                "FAX",
                "RIDE",
                "CUT",
                "CUZ",
                "WOW",
                "BEAT",
                "BEAM",
                "SAVE",
                "EAT",
                "JOBS",
                "JETS",
                "ITM",
                "AGE",
                "AG",
                "GEM",
                "EMO",
                "F",
                "MOM",
                "MOMO",
                "MOAT",
                "LOAN",
                "CARS",
                "GLAD",
                "LAD",
                "OIL",
                "OI",
                "ROLL",
                "CEO",
                "SELF",
                "ELF",
                "USA",
                "UK",
                "MEN",
                "MED",
                "MEDS",
                "FAT",
                "FATE",
                "LEAD",
                "LEE",
                "CFO",
                "RIOT",
                "RIO",
                "SIZE",
                "SIRI",
                "SILK",
                "ZEN",
                "LEAP",
                "PG",
                "API",
                "PILL",
                "LEND",
                "APPS",
                "PSX",
                "PSA",
                "PS",
                "PM",
                "FORM",
                "FOX",
                "OXY",
                "FORD",
                "PLUG",
                "STEP",
                "FIT",
                "DOG",
                "DOC",
                "LAND",
                "TEAM",
                "AMC",
                "MCD",
                "MC",
                "FLY",
                "FLUX",
                "LYFT",
                "FLEE",
                "YUM",
                "FLOW",
                "LOOP",
                "IRL",
                "WOOD",
                "PEAK",
                "SON",
                "SOLO",
                "SOL",
                "AIR",
                "KEY",
                "KEYS",
                "EYE",
                "YETI",
                "YELP",
                "BUD",
                "BUG",
                "SHOP",
                "SHO",
                "USD",
                "USB",
                "SD",
                "DRIP",
                "EYES",
                "AI",
                "SPOT",
                "BOOM",
                "MGMT",
                "BOIL",
                "BOOT",
                "BOUT",
                "U",
                "UU",
                "UUU",
                "UUUUU",
                "BOSS",
                "SIX",
                "SITE",
                "FOUR",
                "ALL",
                "BAR",
                "BAND",
                "TOWN",
                "TOK",
                "TIP",
                "PAYS",
                "PAVE",
                "PAR",
                "TAN",
                "TAP",
                "AP",
                "APT",
                "TV",
                "VOO",
                "PRO",
                "PROF",
                "MAR",
                "AA",
                "PROS",
                "EDIT",
                "DIM",
                "DIAL",
                "ED",
                "DIV",
                "DIG",
                "GPS",
                "BJ",
                "DIS",
                "DISH",
                "GF",
                "DOOR",
                "FIVE",
                "FB",
                "SUM",
                "SUMO",
                "SUP",
                "ROAD",
                "BOND",
                "NC",
                "NM",
                "TEN",
                "MASS",
                "ALOT",
                "LAZY",
                "LAWS",
                "ROCK",
                "CIA",
                "BOX",
                "JOE",
                "LEG",
                "AUTO",
                "TOUR",
                "SHIP",
                "HIPS",
                "HI",
                "C",

            };
         }
    }
}