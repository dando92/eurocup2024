import { Tab } from "@headlessui/react";
import PlayersList from "../components/manage/players/PlayersList";
import SongsList from "../components/manage/songs/SongsList";
import TournamentSettings from "../components/manage/tournament/TournamentSettings";

export function classNames(...classes: string[]) {
  return classes.filter(Boolean).join(" ");
}

export default function ManagePage() {
  return (
    <div>
      <h1 className="text-3xl text-center">Tournament settings</h1>
      <Tab.Group>
        <Tab.List className="flex flex-row gap-10 border-b mt-5">
          <Tab
            className={({ selected }) =>
              classNames(
                "py-2 px-4 text-lg",
                selected ? "border-b-2 border-blue-500 font-bold text-blue-500" : "text-gray-500"
              )
            }
          >
            General
          </Tab>
          <Tab
            className={({ selected }) =>
              classNames(
                "py-2 px-4 text-lg",
                selected ? "border-b-2 border-blue-500 font-bold text-blue-500" : "text-gray-500"
              )
            }
          >
            Songs
          </Tab>
          <Tab
            className={({ selected }) =>
              classNames(
                "py-2 px-4 text-lg",
                selected ? "border-b-2 border-blue-500 font-bold text-blue-500" : "text-gray-500"
              )
            }
          >
            Players
          </Tab>
        </Tab.List>
        <Tab.Panels className="mt-3">
          <Tab.Panel>
            <TournamentSettings />
          </Tab.Panel>
          <Tab.Panel>
            <SongsList />
          </Tab.Panel>
          <Tab.Panel>
            <PlayersList />
          </Tab.Panel>
        </Tab.Panels>
      </Tab.Group>
    </div>
  );
}
