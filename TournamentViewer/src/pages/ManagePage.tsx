import { Tab, TabGroup, TabList, TabPanels, TabPanel } from "@headlessui/react";
import PlayersList from "../components/manage/players/PlayersList";
import SongsList from "../components/manage/songs/SongsList";
import TournamentSettings from "../components/manage/TournamentSettings";

export default function ManagePage() {
  return (
    <div >
      <h1 className="text-3xl text-middle">Tournament settings</h1>
      {/* Three tabs: Tournament, Songs, Players with headlessui */}
      <TabGroup className="text-lg mt-5">
        <TabList className="flex flex-row gap-10 border-b">
          <Tab className={({selected}) => `${selected ? "border-b border-blu font-bold text-blu" : ""}`}>General</Tab>
          <Tab className={({selected}) => `${selected ? "border-b border-blu font-bold text-blu" : ""}`}>Songs</Tab>
          <Tab className={({selected}) => `${selected ? "border-b border-blu font-bold text-blu" : ""}`}>Players</Tab>
        </TabList>
        <TabPanels className="mt-3">
          <TabPanel><TournamentSettings /></TabPanel>
          <TabPanel><SongsList /></TabPanel>
          <TabPanel><PlayersList /></TabPanel>
        </TabPanels>
      </TabGroup>
    </div>
  );
}
