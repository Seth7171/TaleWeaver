// Filename: WordGenerator.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Handles randomal riddles answer. (new riddle system)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordGenerator : MonoBehaviour
{

    // Array of 500 diverse words
    private string[] words = new string[]
    {
        "Hunger", "Fear", "Tears", "Unicorn", "Moonlight", "Echo", "Thunder", "Shadow", "Memory", "Dream",
        "Whisper", "Storm", "Flame", "Ocean", "Silence", "Breeze", "Frost", "Aurora", "Mirage", "Chaos",
        "Rain", "Mystery", "Cloud", "Phoenix", "Infinity", "Dusk", "Dawn", "Dragon", "Serenity", "Magic",
        "Illusion", "Fable", "Harmony", "Whirlwind", "Labyrinth", "Sorcery", "Spell", "Chill", "Twilight",
        "Comet", "Eclipse", "Glacier", "Vortex", "Starlight", "Nebula", "Galaxy", "Universe", "Meteor",
        "Blizzard", "Avalanche", "Icicle", "Tempest", "Flood", "Tornado", "Hurricane", "Earthquake", "Volcano",
        "Mountain", "River", "Forest", "Cave", "Valley", "Island", "Desert", "Canyon", "Sea", "Lake", "Stream",
        "Waterfall", "Lagoon", "Wave", "Tide", "Current", "Swamp", "Meadow", "Prairie", "Glade", "Grove",
        "Woodland", "Garden", "Orchard", "Field", "Plateau", "Hill", "Peak", "Summit", "Cliff", "Ridge",
        "Pinnacle", "Basin", "Gorge", "Ravine", "Gully", "Abyss", "Crevice", "Rift", "Fjord", "Bay", "Gulf",
        "Cove", "Harbor", "Dock", "Quay", "Jetty", "Pier", "Bridge", "Tunnel", "Arch", "Viaduct", "Ramp",
        "Flyover", "Roundabout", "Junction", "Crossroad", "Fork", "Curve", "Bend", "Spiral", "Twist", "Rock",
        "Sway", "Swing", "Shake", "Tremble", "Vibrate", "Rumble", "Roar", "Buzz", "Hum", "Whistle", "Whoosh",
        "Rustle", "Snap", "Bang", "Boom", "Clap", "Rattle", "Clink", "Clang", "Thud", "Thump", "Whack", "Slap",
        "Punch", "Kick", "Stomp", "Crash", "Collide", "Smash", "Shatter", "Burst", "Ignite", "Glow", "Flare",
        "Shine", "Sparkle", "Twinkle", "Glimmer", "Gleam", "Shimmer", "Glitter", "Spark", "Flash", "Dazzle",
        "Glare", "Beam", "Radiant", "Luminous", "Aura", "Halo", "Crescent", "Corona", "Solar", "Lunar", "Stellar",
        "Orbit", "Cosmos", "Void", "Darkness", "Star", "Planet", "Moon", "Sun", "Earth", "Mars", "Jupiter",
        "Saturn", "Venus", "Mercury", "Neptune", "Uranus", "Pluto", "Alien", "Spacecraft", "Rocket", "Satellite",
        "Astro", "Spaceship", "Meteorite", "Atmosphere", "Vacuum", "Celestial", "Telescope", "Astronaut", "Explorer",
        "Orbit", "Galaxy", "Quasar", "Meteor", "Comet", "Nebula", "Cosmic", "Solar", "Lunar", "Stellar", "Planetary",
        "Astral", "Galactic", "Universal", "Infinite", "Blackhole", "Wormhole", "Dimension", "Parallel", "Multiverse",
        "Time", "Continuum", "Relativity", "Quantum", "Singularity", "BigBang", "Event", "Horizon", "Particle", "Atom",
        "Photon", "Neutron", "Electron", "Proton", "Quark", "Boson", "Hadron", "Lepton", "Baryon", "Fermion", "Neutrino",
        "Gravity", "Mass", "Energy", "Wave", "Field", "Charge", "Force", "Interaction", "Symmetry", "Spin", "Uncertainty",
        "Principle", "Theory", "Equation", "Experiment", "Observation", "Measurement", "Prediction", "Model", "Simulation",
        "Analysis", "Data", "Information", "Knowledge", "Understanding", "Wisdom", "Insight", "Discovery", "Innovation",
        "Invention", "Technology", "Engineering", "Science", "Research", "Development", "Application", "Function", "Design",
        "Structure", "System", "Process", "Mechanism", "Method", "Technique", "Procedure", "Protocol", "Standard", "Rule",
        "Law", "Principle", "Concept", "Idea", "Thought", "Hypothesis", "Postulate", "Axiom", "Lemma", "Theorem", "Corollary",
        "Proposition", "Proof", "Argument", "Reason", "Logic", "Rationale", "Explanation", "Interpretation", "Analysis",
        "Evaluation", "Assessment", "Judgment", "Opinion", "Belief", "Attitude", "Perspective", "Viewpoint", "Stance",
        "Position", "Approach", "Strategy", "Plan", "Tactic", "Policy", "Procedure", "Routine", "Habit", "Custom",
        "Tradition", "Practice", "Habit", "Pattern", "Trend", "Tendency", "Movement", "Change", "Transformation",
        "Evolution", "Progress", "Advancement", "Growth", "Development", "Maturity", "Aging", "Senescence", "Decline",
        "Deterioration", "Degradation", "Decay", "Death", "Extinction", "Annihilation", "Existence", "Life", "Survival",
        "Continuity", "Persistence", "Duration", "Eternity", "Perpetuity", "Immortality", "Infinity", "Timelessness",
        "Endlessness", "Boundlessness", "Limitlessness", "Vastness", "Immensity", "Magnitude", "Grandeur", "Majesty",
        "Sublimity", "Glory", "Splendor", "Resplendence", "Radiance", "Brilliance", "Luster", "Gleam", "Sheen", "Gloss",
        "Polish", "Shine", "Glow", "Shimmer", "Glimmer", "Glitter", "Sparkle", "Twinkle", "Flicker", "Flare", "Flash",
        "Blaze", "Inferno", "Conflagration", "Wildfire", "Bonfire", "Campfire", "Hearth", "Fireplace", "Chimney",
        "Stove", "Furnace", "Kiln", "Forge", "Smelter", "Incinerator", "Combustor", "Burner", "Torch", "Lantern",
        "Candle", "Lamp", "Light", "Bulb", "Glowstick", "Flashlight", "Headlamp", "Spotlight", "Searchlight", "Beacon",
        "Signal", "Indicator", "Marker", "Flag", "Sign", "Symbol", "Emblem", "Badge", "Insignia", "Seal", "Stamp",
        "Logo", "Trademark", "Brand", "Label", "Tag", "Ticket", "Pass", "Permit", "License", "Certification", "Diploma",
        "Degree", "Qualification", "Credential", "Accreditation", "Recognition", "Award", "Prize", "Trophy", "Medal",
        "Ribbon", "Honor", "Distinction", "Fame", "Renown", "Reputation", "Prestige", "Esteem", "Respect", "Admiration",
        "Adulation", "Worship", "Reverence", "Veneration", "Awe", "Wonder", "Amazement", "Astonishment", "Surprise",
        "Shock", "Stun", "Jolt", "Jar", "Shake", "Rattle", "Vibrate", "Oscillate", "Wobble", "Jiggle", "Wiggle",
        "Flick", "Flip", "Snap", "Crack", "Pop", "Bang", "Boom", "Clap", "Slap", "Hit", "Strike", "Punch", "Kick",
        "Stomp", "Tread", "Trample", "Crush", "Squash", "Smash", "Bash", "Whack", "Swipe", "Lash", "Flog", "Whip",
        "Beat", "Pound", "Hammer", "Drill", "Pierce", "Stab", "Slash", "Cut", "Slice", "Chop", "Hack", "Sever",
        "Cleave", "Split", "Fracture", "Break", "Shatter"
    };

    private System.Random random = new System.Random();

    // Function to get a random word from the array
    public string GetRandomWord()
    {
        return words[random.Next(words.Length)];
    }
}

